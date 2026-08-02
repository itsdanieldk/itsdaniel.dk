namespace Yggdrasil.Web.Views

open Yggdrasil.Web
open Yggdrasil.Content

open Giraffe.ViewEngine

module Note =

    let private txt = encodedText

    let index (config: SiteConfig) (byYear: (int * Note list) list) =
        let meta = config.Page "notes"
        let body =
            Components.container [
                div [ _class "space-y-10" ] [
                    Components.pageHeading [ txt "Notes" ]
                    p [ _class "animate"; Components.stagger 1 ] [ txt "Thoughts on functional programming, software architecture, and lessons learned along the way." ]
                    div [ _class "space-y-4" ] [
                        for i, (year, notes) in List.indexed byYear do
                            section [ _class "animate space-y-4"; Components.stagger (i + 2) ] [
                                div [ _class "text-sm font-semibold uppercase tracking-wider text-black/40 dark:text-white/40" ] [ txt (string year) ]
                                div [] [
                                    ul [ _class "flex flex-col gap-4" ] [
                                        for note in notes -> li [] [ Components.arrowCard (FeedEntry.Note note) ]
                                    ]
                                ]
                            ]
                    ]
                ]
            ]

        Layouts.context config "/notes" (Some meta.Title) (Some meta.Description) (Some "notes") None "website" [ body ]

    let show (config: SiteConfig) (note: Note) (prev: Note option) (next: Note option) =
        let body = ArticleLayout.articlePage (FeedEntry.Note note) (Option.map FeedEntry.Note prev) (Option.map FeedEntry.Note next) "/notes" "Back to notes" []

        let article =
            { Published = note.Date
              Modified = defaultArg note.UpdatedDate note.Date
              Tags = note.Tags }

        { Layouts.articleContext config $"/notes/{note.Id}" (Some note.Title) (Some note.Description) (Some "notes") (Some(JsonLd.note config note)) article body with
            OgImage = Some(Site.ogImagePath "notes" note.Id) }
