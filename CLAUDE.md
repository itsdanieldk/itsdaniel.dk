# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

- `pnpm dev` — start dev server (`pnpm dev:network` to expose on LAN)
- `pnpm build` — runs `astro check` then builds to `dist/`
- `pnpm preview` — serve production build locally
- `pnpm lint` / `pnpm lint:fix` — ESLint
- `pnpm format` / `pnpm format:check` — Prettier

There is no test runner. Use `pnpm build` as the primary validation step.

## Architecture

Astro 6 static site deployed to Vercel. Two content collections (notes, projects) with Markdown/MDX, Tailwind CSS v4, and TypeScript in strict mode.

**Routing**: file-based from `src/pages/`. Dynamic routes use `[...slug].astro` with `getStaticPaths()`.

**Content collections**: schemas in `src/content.config.ts` (Zod). Collection helpers (`getAllNotes`, `getAllProjects`, `groupNotesByYear`) in `src/lib/collections.ts`. Drafts are filtered at query time, not build time.

**Layout**: single `PageLayout.astro` wraps all pages with `Head`, `Header`, `Footer`, and Vercel Analytics.

**Styling**: Tailwind v4 configured CSS-first in `src/styles/global.css` (no `tailwind.config`). Wired through `@tailwindcss/vite` in `astro.config.mjs`. Typography plugin loaded via `@plugin` directive. Dark mode uses a `.dark` class on `<html>`, toggled by inline script in `Head.astro` with `localStorage`.

**SEO**: JSON-LD structured data embedded in page templates. Open Graph, Twitter cards, canonical URLs, RSS feed (`rss.xml.ts`).

**View transitions**: Astro `ClientRouter` enabled. `Head.astro` handles `astro:before-swap` and `astro:after-swap` lifecycle events.

**Security headers**: configured in `vercel.json` (CSP, HSTS, X-Frame-Options, etc.).

## Key Conventions

- **Path aliases**: `@*` maps to `src/*`. Import as `@components/Foo.astro`, `@lib/utils`, `@consts`, `@types`.
- **Site constants**: `src/consts.ts` holds site metadata. Types are in `src/types.ts`.
- **Class merging**: use `cn()` from `src/lib/utils.ts` (wraps `clsx` + `tailwind-merge`) for conditional Tailwind classes.
- **Animations**: `.animate` CSS class gives staggered fade-in on load; `prefers-reduced-motion` is respected.
- **Code highlighting**: Shiki with `catppuccin-macchiato` theme.
- **Fonts**: Fira Sans (body), Fira Code Variable (mono), Metamorphous (titles) via `@fontsource` packages.

## Content Authoring

Notes and projects live in `src/content/notes/` and `src/content/projects/` as Markdown/MDX.

**Notes frontmatter** (required: `title`, `description`, `date`):
```yaml
title: "Post Title"
description: "Brief description"
date: 2025-01-15
updatedDate: 2025-02-01 # optional
draft: false            # optional
tags: ["fp", "dotnet"]  # optional, max 3
```

**Projects frontmatter** (adds optional `demoURL`, `repoURL`).

- Tags are optional but capped at **3 per item** (enforced by Zod schema).
- Use kebab-case for content folder/slug names. Set `draft: true` to exclude from builds and feeds.
- Drafts are filtered at query time in `src/lib/collections.ts`.

## Coding Style

- **Prettier**: 2-space indent, semicolons, double quotes, `printWidth` 140, LF, `trailingComma: "es5"`.
- **ESLint**: flat config for JS/TS and Astro. Enforces semicolons and double quotes.
- **TypeScript**: strict mode with `strictNullChecks` (extends `astro/tsconfigs/strict`).
