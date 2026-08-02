---
title: "Yggdrasil"
description: "The F# static-site generator that builds this website."
date: 2026-07-28
tags: ["F#", ".NET", "static site generator"]
repoURL: "https://github.com/itsdanieldk/yggdrasil"
---

**Yggdrasil** is the static-site generator that builds the site you're reading. It takes the Markdown and YAML in `content/` and the files in `static/`, and renders a portable `dist/` folder — every page, feed, share card and stylesheet — with nothing running at request time. It's named for the world tree of Norse myth, which is the honest description of what it does: the content tree grows into the route tree.

I didn't set out to write my own. This site has been through a couple of stacks, and each time the interesting part was never the rendering — it was the content model.

## Key Features

- **Content as a validated domain model** — Markdown and YAML frontmatter decode into F# records, and a bad file fails the build with a message naming the file and the field
- **Routes as one exhaustive type** — every URL, output path, content type and renderer derives from a single discriminated union
- **Dual-theme syntax highlighting** — one pass of TextMate grammars emits both light and dark palettes, switched in CSS with no client-side work
- **Generated share cards** — 1200×630 Open Graph images drawn per page with SkiaSharp at build time
- **No Node in the toolchain** — the build downloads pinned standalone Tailwind and esbuild binaries and caches them
- **Fully tested** — the Expecto suite runs against the real content, not fixtures

## One Command

```bash
dotnet run --project src/Yggdrasil.Generate
```

That loads and validates all content, recreates `dist/`, copies `static/`, builds minified CSS and JS, draws the share cards, and renders every route in parallel. The only requirement is the .NET SDK.

Validation collects *every* error before it gives up, so a batch of edits produces one list to work through instead of a dozen rebuild cycles.

## Routes Are a Single Type

This is the part that made writing it worthwhile — every route the site can produce is one case:

```fsharp
type Route =
    | Home
    | About
    | NotesIndex
    | NoteShow of note: Note * prev: Note option * next: Note option
    | ProjectsIndex
    | ProjectShow of project: Project * prev: Project option * next: Project option
    | FragrancesIndex
    | TagsIndex
    | TagShow of tag: string * notes: Note list * projects: Project list
    | NotFound
    | Rss
    | SitemapIndex
    | Sitemap
    | Robots
    | Webmanifest
```

The canonical URL, the output path on disk, the content type and the renderer are each a `match` over that type — `404.html` included, as `NotFound`. Adding a route means adding a case, and the compiler then walks me through every function that needs to handle it. The parameterised cases fan out into a page per note, project and tag.

Views are [Giraffe.ViewEngine](https://github.com/giraffe-fsharp/Giraffe.ViewEngine) — HTML as F# values, so a layout is a function and a component is a function, type-checked like everything else.

## The Build Downloads Its Own Tools

There's no `package.json`. Tailwind and esbuild are real dependencies, but they ship as standalone binaries, so the generator fetches the pinned versions on first run and caches them under `.bin/`:

| Tool         | Version | Purpose                                            |
| ------------ | ------- | -------------------------------------------------- |
| Tailwind CLI | 4.3.3   | Scans the F# view sources and emits the stylesheet |
| esbuild      | 0.28.1  | Bundles and minifies the client script             |

Tailwind scanning F# source files is less strange than it sounds — the class strings live in the view functions, so pointing `@source` at `src/Yggdrasil.Web` is all it takes.

It's more code than a config file for an off-the-shelf generator, and that's the trade. What I get back is a content model that's checked at compile time, errors that arrive all at once and point at a line, and a build with one dependency I actually have to install. The full source is in the [repository](https://github.com/itsdanieldk/yggdrasil).
