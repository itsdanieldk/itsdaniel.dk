# Yggdrasil

Static-site generator for [itsdaniel.dk](https://itsdaniel.dk), written in F#. Reads the Markdown and
YAML in [`content/`](content) plus the files in [`static/`](static), and renders a portable `dist/` —
every page, style and script, with no server at runtime. Any static host works; the deploy target
here is Vercel.

## Requirements

.NET 10 SDK, pinned in `global.json`. No Node: the generator downloads the standalone **Tailwind CLI
4.3.3** and **esbuild 0.28.1** binaries on first run and caches them under `.bin/`.

Supported platforms are bounded by Tailwind's standalone releases — macOS, Linux (glibc and musl) and
Windows, on x64 and arm64. Anything else fails immediately naming the detected platform rather than
downloading a binary that cannot run. CI runs the full generate on Ubuntu, macOS and Windows.

## Build

```sh
dotnet run --project src/Yggdrasil.Generate
```

Validates all content (listing every error at once), recreates `dist/`, copies `static/**`, builds
minified CSS/JS, renders the 15 route types — notes, projects and tags expand per item — plus
`404.html`, then verifies every emitted `href`/`src` resolves. Paths resolve from `global.json`, so
the command works from any working directory.

Preview the output:

```sh
python3 -m http.server 8799 --directory dist
```

A plain file server redirects `/notes/foo` → `/notes/foo/`; Vercel serves the canonical no-slash form
via `vercel.json` (`cleanUrls`, `trailingSlash: false`). Both resolve either way.

## Configuration

Build-time environment variables, all validated in
[`Config.fs`](src/Yggdrasil.Generate/Config.fs) before anything is written.

| Variable | Default | Purpose |
|---|---|---|
| `SITE_URL` | `site.yaml`'s `url` | Base URL for canonical links, Open Graph and feeds. Must be an absolute `http(s)` URL; the trailing slash is normalised. |
| `SKIP_ASSETS` | _(unset)_ | Any value skips the CSS/JS **and OG-card** build. The HTML still references them, so output is incomplete — fast iteration only. |
| `SITE_ROOT` | auto, via `global.json` | Project root override, or the first CLI argument. Must contain `global.json` and `site.yaml` — checked because the generator deletes `dist/` beneath it. |

## Tests

```sh
dotnet test                                   # VSTest adapter; what CI runs
dotnet run --project tests/Yggdrasil.Tests    # Expecto console app
dotnet test --collect:"XPlat Code Coverage"   # Cobertura under TestResults/
```

The console app forwards Expecto arguments after `--`, e.g. `-- --filter-test-list feeds`. A new
`[<Tests>]` module must be registered in `Main.fs`, or only the VSTest adapter will find it.

`Yggdrasil.Content` and `Yggdrasil.Web` sit around 90–96% line coverage; `Yggdrasil.Generate` reads
lower because `Assets` downloads binaries and runs external processes.

## Layout

```
content/                   Markdown/YAML source (+ source PNGs)
static/                    copied verbatim into dist/
assets/css, assets/js      Tailwind and esbuild inputs
assets/grammars/           TextMate grammars + theme — see its README
src/Yggdrasil.Content/     domain and content pipeline
src/Yggdrasil.Web/         views, components, layouts, feeds, routes
src/Yggdrasil.Generate/    the entry point you run
tests/Yggdrasil.Tests/     Expecto suite
```

`dist/` and `.bin/` are build output and gitignored.

## Making it yours

Identity lives in data — pointing this at someone else needs no `.fs` edit:

- **`site.yaml`** — name, author, tagline, URL, avatar, socials, and each index page's title and
  description. Unknown keys are an error, so a typo fails the build.
- **`content/pages/home/index.md` and `content/pages/about/index.md`** — required; the build fails
  without them. Frontmatter carries the title, description, heading and optional emoji.
- **`static/avatar*.{png,webp}` and `static/favicon/`** — paths come from `site.yaml`, so nothing
  else needs editing.

Layout, components, colours and route structure are the theme, and are meant to be edited in code.
`content/fragrances/` is a personal collection you'll want to delete.

## Deploying

The site is built in CI and uploaded **prebuilt** — Vercel's build image has no .NET, so the
generator cannot run there. The `deploy` job in [`ci.yml`](.github/workflows/ci.yml) builds on a
runner with the SDK and ships with `vercel deploy --prebuilt`. It is gated on `needs: [test,
generate]`, which makes CI the only path to production: a commit that fails `dotnet test` cannot
deploy.

One-time setup:

1. Repository secrets `VERCEL_TOKEN`, `VERCEL_ORG_ID` and `VERCEL_PROJECT_ID` — the latter two from
   `.vercel/project.json` after `vercel link`, or the project settings page.
2. Turn off the Git integration's production deploys, or every push deploys twice — once ungated by
   Vercel, which would fail anyway for lack of `dotnet`.
3. Set `SITE_URL` in the Vercel environment if it differs from `site.yaml`'s `url`.

> **Do not hand-write `.vercel/output/config.json`.** `vercel build` is what translates `cleanUrls`,
> `trailingSlash` and `headers` from `vercel.json` into Build Output API routes; that format supports
> none of those keys, so translating by hand silently drops the CSP and HSTS headers. The deploy
> job's smoke test curls the deployment and fails if they did not survive.

To roll back, promote the previous deployment from the Vercel dashboard — it serves an already-built
artifact and takes effect immediately. Revert the commit afterwards: restore the site first, fix the
repository second.

## Gotchas

- **An unhighlightable code fence fails the build**, a typo'd label included. Supported labels are
  `fsharp`/`fs`/`f#`, `scala` and `bash`/`shell`/`sh`/`shellscript` — only the languages this site
  actually uses. Leave a fence untagged for a deliberately plain block; to add a language see
  [`assets/grammars/README.md`](assets/grammars/README.md).
- Highlighting emits light colours inline and dark ones as `--tm-dark*` variables that `app.css`
  promotes under `html.dark`. Both slots are Catppuccin Frappé today (`Themes` in `Highlight.fs`), so
  a code block looks the same in either site theme — which is why the copy button is styled
  light-on-dark unconditionally.
- **Source PNGs must live next to their Markdown** in `content/`. The image rewrite reads their real
  dimensions at build time; the rendered `<img>` points at the prebuilt `.webp` under
  `static/images/`.
- `assets/css/app.css` carries two edits over a stock Tailwind entry: the `@source` scan of
  `src/Yggdrasil.Web`, and the dark-mode block targeting the `.tm` highlight wrapper.
- The footer year is frozen at generate time; rebuild to refresh it.
- **OG share cards render from TTF, not the WOFF2 the site serves.** `SkiaSharp.NativeAssets.Linux.NoDependencies`
  ships without Brotli, so `SKTypeface.FromFile` cannot decode WOFF2 on Linux. The three faces the
  cards use live as lossless TTF conversions under `assets/fonts/` — a build input, never copied into
  `dist/`. macOS Skia would read the WOFF2 directly; TTF keeps the Linux CI generate working too.

## Licensing

- **The generator is MIT** — `src/`, `tests/`, `assets/`, build configuration and docs. Take it and
  build your own site with it. See [`LICENSE.md`](LICENSE.md).
- **The content is not** — notes, project write-ups, fragrance reviews and the avatar are reserved;
  [`LICENSE-CONTENT.md`](LICENSE-CONTENT.md) has the exact paths. Replace `content/` with your own
  and you are clear. `site.yaml` is yours to edit.
- **Fragrance photographs** under `static/images/fragrances/` are third-party product shots, not mine
  to sublicense. A fork should delete them.

Redistributed third-party files carry attribution beside themselves:
[`assets/grammars/README.md`](assets/grammars/README.md) for the grammars and theme, and
`static/fonts/OFL.txt` for the fonts — which ships at `/fonts/OFL.txt`, as the SIL OFL requires.
Everything else is a build-time NuGet dependency, never redistributed, so no attribution attaches.
