namespace Yggdrasil.Content

open System
open System.IO
open System.Text
open System.Collections.Generic
open System.Collections.Concurrent

open TextMateSharp.Themes
open TextMateSharp.Registry
open TextMateSharp.Grammars
open TextMateSharp.Internal.Themes.Reader
open TextMateSharp.Internal.Grammars.Reader

type FallbackEvent =
    { SourcePath: string
      Language: string
      Reason: string }

module private Themes =
    let light = "catppuccin-frappe.json"
    let dark = "catppuccin-frappe.json"

type private GrammarRegistryOptions(grammarDir: string, grammarFiles: IDictionary<string, string>) =

    let readTheme (file: string) =
        use reader = new StreamReader(Path.Combine(grammarDir, file))
        ThemeReader.ReadThemeSync reader

    member _.ReadThemeFile (file: string) =
        readTheme file

    interface IRegistryOptions with
        member _.GetGrammar scopeName =
            match grammarFiles.TryGetValue scopeName with
            | true, file ->
                use reader = new StreamReader(Path.Combine(grammarDir, file))
                GrammarReader.ReadGrammarSync reader
            | _ -> null

        member _.GetInjections _scopeName =
            null

        member _.GetTheme _scopeName =
            null

        member _.GetDefaultTheme() =
            readTheme Themes.light

module Highlight =

    let private scopeByLang =
        Map
            [ "fsharp", "source.fsharp"
              "fs", "source.fsharp"
              "f#", "source.fsharp"
              "scala", "source.scala"
              "bash", "source.shell"
              "shell", "source.shell"
              "sh", "source.shell"
              "shellscript", "source.shell" ]

    let supportedLanguages =
        scopeByLang
        |> Map.toList
        |> List.map fst
        |> List.sort

    let private grammarFiles =
        dict
            [ "source.fsharp", "fsharp.tmLanguage.json"
              "source.scala", "scala.tmLanguage.json"
              "source.shell", "shell.tmLanguage.json" ]

    type Highlighter =
        private
            { Registry: Registry
              Light: Theme
              Dark: Theme
              DefaultFgLight: string
              DefaultFgDark: string
              DefaultBgLight: string
              DefaultBgDark: string
              Grammars: ConcurrentDictionary<string, IGrammar option>
              Fallbacks: ConcurrentQueue<FallbackEvent> }

    let private guiColor (theme: Theme) (key: string) (fallback: string) =
        let dict = theme.GetGuiColorDictionary()
        if not (isNull dict) && dict.ContainsKey key then
            dict.[key]
        else
            fallback

    let create (grammarDir: string) =
        let options = GrammarRegistryOptions(grammarDir, grammarFiles)
        let registry = Registry(options :> IRegistryOptions)
        let light = Theme.CreateFromRawTheme(options.ReadThemeFile Themes.light, options)
        let dark = Theme.CreateFromRawTheme(options.ReadThemeFile Themes.dark, options)

        let grammars = ConcurrentDictionary<string, IGrammar option>()

        for scope in grammarFiles.Keys do
            let grammar = registry.LoadGrammar scope
            grammars.[scope] <-
                if isNull (box grammar) then
                    None
                else
                    Some grammar

        { Registry = registry
          Light = light
          Dark = dark
          DefaultFgLight = guiColor light "editor.foreground" "#c6d0f5"
          DefaultFgDark = guiColor dark "editor.foreground" "#c6d0f5"
          DefaultBgLight = guiColor light "editor.background" "#303446"
          DefaultBgDark = guiColor dark "editor.background" "#303446"
          Grammars = grammars
          Fallbacks = ConcurrentQueue<FallbackEvent>() }

    let fallbacks (h: Highlighter) =
        List.ofSeq h.Fallbacks

    let private grammarFor (h: Highlighter) (scope: string) =
        match h.Grammars.TryGetValue scope with
        | true, g -> g
        | _ -> None

    let private scopeMatches (selector: string) (scope: string) =
        scope = selector
        || scope.Length > selector.Length
           && scope.StartsWith(selector, StringComparison.Ordinal)
           && scope.[selector.Length] = '.'

    let private ruleApplies (rule: ThemeTrieElementRule) (scopesInnerFirst: string list) =
        if isNull (box rule.parentScopes) || rule.parentScopes.Count = 0 then
            true
        else
            match scopesInnerFirst with
            | own :: ancestors when scopeMatches rule.parentScopes.[0] own ->
                let mutable remaining = ancestors
                let mutable i = 1
                let mutable ok = true

                while ok && i < rule.parentScopes.Count do
                    let selector = rule.parentScopes.[i]

                    match remaining |> List.skipWhile (fun s -> not (scopeMatches selector s)) with
                    | [] -> ok <- false
                    | _ :: outer ->
                        remaining <- outer
                        i <- i + 1

                ok
            | _ -> false

    let private resolveStyle (theme: Theme) (scopes: List<string>) =
        let mutable fg = -1
        let mutable style = FontStyle.NotSet
        let scopesInnerFirst = scopes |> List.ofSeq |> List.rev

        for rule in theme.Match scopes do
            if ruleApplies rule scopesInnerFirst then
                if fg = -1 && rule.foreground > 0 then
                    fg <- rule.foreground

                if style = FontStyle.NotSet && rule.fontStyle <> FontStyle.NotSet then
                    style <- rule.fontStyle

        (if fg > 0 then Some(theme.GetColor fg) else None), style

    let private escapeInto (sb: StringBuilder) (s: string) =
        for c in s do
            match c with
            | '&' -> sb.Append "&amp;" |> ignore
            | '<' -> sb.Append "&lt;" |> ignore
            | '>' -> sb.Append "&gt;" |> ignore
            | c -> sb.Append c |> ignore

    let private styleDecls (style: FontStyle) =
        if int style <= 0 then
            ""
        else
        [ if style.HasFlag FontStyle.Italic then
            "font-style:italic"
          if style.HasFlag FontStyle.Bold then
            "font-weight:bold"
          if style.HasFlag FontStyle.Underline && style.HasFlag FontStyle.Strikethrough then
              "text-decoration:underline line-through"
          elif style.HasFlag FontStyle.Underline then
              "text-decoration:underline"
          elif style.HasFlag FontStyle.Strikethrough then
              "text-decoration:line-through" ]
        |> String.concat ";"

    let private emitToken
        (sb: StringBuilder)
        (h: Highlighter)
        (text: string)
        (lightFg: string option)
        (darkFg: string option)
        (style: FontStyle) =
        let distinctColour =
            match lightFg with
            | Some c when not (String.Equals(c, h.DefaultFgLight, StringComparison.OrdinalIgnoreCase)) ->
                Some c
            | _ ->
                None

        let styles = styleDecls style
        let hasStyle = styles <> ""

        match distinctColour, hasStyle with
        | None, false -> escapeInto sb text
        | _ ->
            let decls =
                [ match distinctColour with
                  | Some c ->
                      yield $"color:{c}"
                      match darkFg with
                      | Some d -> yield $"--tm-dark:{d}"
                      | None -> ()
                  | None -> ()
                  if hasStyle then yield styles ]
                |> String.concat ";"

            sb.Append("<span style=\"").Append(decls).Append "\">" |> ignore
            escapeInto sb text
            sb.Append "</span>" |> ignore

    let private openBlock (sb: StringBuilder) (h: Highlighter) =
        sb
            .Append("<pre class=\"tm\" style=\"background-color:")
            .Append(h.DefaultBgLight)
            .Append(";--tm-dark-bg:")
            .Append(h.DefaultBgDark)
            .Append("\"><code style=\"color:")
            .Append(h.DefaultFgLight)
            .Append(";--tm-dark:")
            .Append(h.DefaultFgDark)
            .Append("\">")
        |> ignore

    let plainBlock (h: Highlighter) (code: string) =
        let sb = StringBuilder()
        openBlock sb h
        escapeInto sb (code.Replace("\r\n", "\n").TrimEnd '\n')
        sb.Append "</code></pre>" |> ignore
        sb.ToString()

    let private highlightWith (h: Highlighter) (grammar: IGrammar) (code: string) =
        let sb = StringBuilder()
        openBlock sb h
        let normalized = code.Replace("\r\n", "\n")

        let trimmed =
            if normalized.EndsWith "\n" then normalized.[.. normalized.Length - 2] else normalized

        let lines = trimmed.Split '\n'
        let mutable state: IStateStack = null

        let styleCache = Dictionary<string, string option * string option * FontStyle>()

        let resolveBoth (scopes: List<string>) =
            let key = String.Join(">", scopes)

            match styleCache.TryGetValue key with
            | true, v -> v
            | _ ->
                let lightFg, style = resolveStyle h.Light scopes
                let darkFg, _ = resolveStyle h.Dark scopes
                let v = lightFg, darkFg, style
                styleCache.[key] <- v
                v

        lines
        |> Array.iteri (fun i line ->
            if i > 0 then
                sb.Append '\n' |> ignore

            let result = grammar.TokenizeLine(line, state, TimeSpan.FromSeconds 10.0)
            state <- result.RuleStack

            for token in result.Tokens do
                let startIdx = token.StartIndex
                let endIdx = min token.EndIndex line.Length

                if startIdx < endIdx then
                    let text = line.Substring(startIdx, endIdx - startIdx)
                    let lightFg, darkFg, style = resolveBoth token.Scopes
                    emitToken sb h text lightFg darkFg style)

        sb.Append "</code></pre>" |> ignore
        sb.ToString()

    let highlight (h: Highlighter) (sourcePath: string) (lang: string) (code: string) =
        let langKey =
            (if isNull lang then "" else lang).Trim().ToLowerInvariant()

        let recordFallback reason =
            if langKey <> "" then
                h.Fallbacks.Enqueue
                    { SourcePath = sourcePath
                      Language = langKey
                      Reason = reason }

        match Map.tryFind langKey scopeByLang with
        | None ->
            recordFallback "unknown language"
            plainBlock h code
        | Some scope ->
            match grammarFor h scope with
            | None ->
                recordFallback $"grammar '{scope}' failed to load"
                plainBlock h code
            | Some grammar ->
                try
                    highlightWith h grammar code
                with ex ->
                    recordFallback $"tokenize error: {ex.Message}"
                    plainBlock h code
