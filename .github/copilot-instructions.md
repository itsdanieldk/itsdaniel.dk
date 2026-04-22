# Copilot Instructions

## Build, Lint, and Format Commands

- `pnpm install` — install dependencies (requires Node.js 22.12.0+, pnpm 9+)
- `pnpm dev` — start dev server (`pnpm dev:network` to expose on LAN)
- `pnpm build` — runs `astro check` then builds the site to `dist/`
- `pnpm preview` — serve the production build locally
- `pnpm lint` / `pnpm lint:fix` — run ESLint (auto-fix with `lint:fix`)
- `pnpm format` / `pnpm format:check` — apply or check Prettier formatting
- `pnpm astro <command>` — run Astro CLI tasks (e.g., `pnpm astro check`)

There is no automated test runner. Use `pnpm build` as the primary validation step.

## Architecture

Astro static site deployed to Vercel. Key integration points:

- **Routing**: file-based from `src/pages/`. Dynamic routes use `[...slug].astro` with `getStaticPaths()`.
- **Content collections**: notes (`src/content/notes/`) and projects (`src/content/projects/`), schemas defined in `src/content.config.ts` with Zod. Collection helpers (`getAllNotes`, `getAllProjects`, `groupNotesByYear`) live in `src/lib/collections.ts`.
- **Layouts**: single `PageLayout.astro` wraps all pages with `Head`, `Header`, `Footer`, and Vercel Analytics.
- **Styling**: Tailwind CSS v4 configured CSS-first in `src/styles/global.css` (not `tailwind.config`). Wired through `@tailwindcss/vite` in `astro.config.mjs`. Typography plugin loaded via `@plugin` directive.
- **Theming**: dark/light/system toggle managed by inline `<script>` in `Head.astro` using `localStorage` and a `.dark` class on `<html>`.
- **SEO**: JSON-LD structured data (`WebSite`, `BlogPosting`, `CreativeWork`) embedded in page templates. Open Graph, Twitter cards, canonical URLs, and RSS feed (`rss.xml.ts`) are all set up.
- **Security headers**: configured in `vercel.json` (CSP, HSTS, X-Frame-Options, etc.).

## Key Conventions

- **Path aliases**: `@*` maps to `src/*` (configured in `tsconfig.json`). Import as `@components/Foo.astro`, `@lib/utils`, `@consts`, `@types`.
- **Site constants**: `src/consts.ts` holds site metadata (`SITE`, `HOME`, `NOTES`, `PROJECTS`, `ABOUT`, `SOCIALS`). Types are in `src/types.ts`.
- **Class merging**: use the `cn()` utility from `src/lib/utils.ts` (wraps `clsx` + `tailwind-merge`) for conditional/merged Tailwind classes.
- **Animations**: elements with the `.animate` CSS class get staggered fade-in on page load, with `prefers-reduced-motion` respected.
- **View transitions**: Astro `ClientRouter` is enabled for SPA-style navigation. `Head.astro` handles `astro:before-swap` and `astro:after-swap` lifecycle events.
- **Code highlighting**: Shiki with `catppuccin-macchiato` theme (configured in `astro.config.mjs`).
- **Fonts**: Fira Sans (body), Fira Code Variable (mono), Metamorphous (titles) — imported via `@fontsource` packages in `Head.astro`.

## Content Authoring

Notes and projects are Markdown/MDX files in `src/content/notes/` and `src/content/projects/`.

**Notes frontmatter** (required: `title`, `description`, `date`):

```markdown
---
title: "Post Title"
description: "Brief description"
date: 2025-01-15
draft: false
tags: ["functional programming", "F#", "software design"]
---
```

**Projects frontmatter** (adds optional `demoURL`, `repoURL`):

```markdown
---
title: "Project Name"
description: "Brief description"
date: 2025-01-15
demoURL: "https://example.com"
repoURL: "https://github.com/user/repo"
tags: ["F#", "concurrency", "effect system"]
---
```

- Tags are optional but capped at **3 per item** (enforced by Zod schema).
- Use kebab-case for content folder/slug names (e.g., `src/content/notes/my-article/`).
- Set `draft: true` to exclude content from builds and feeds. Drafts are filtered at query time in `src/lib/collections.ts`.

## Coding Style

- **Prettier**: 2-space indent, semicolons, double quotes, `printWidth` 140, LF line endings, `trailingComma: "es5"`.
- **ESLint**: flat config for JS/TS and Astro files. Enforces semicolons and double quotes.
- **TypeScript**: strict mode with `strictNullChecks` enabled (extends `astro/tsconfigs/strict`).

## Deployment

- Deployed to Vercel. Config in `vercel.json` (security headers) and `astro.config.mjs` (site URL, integrations).
- If adding environment variables, update `src/env.d.ts` and document in `README.md`.
