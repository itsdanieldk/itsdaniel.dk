# 🪻 itsdaniel.dk

Personal blog and portfolio focused on software engineering, functional programming, and distributed systems.

**[itsdaniel.dk](https://itsdaniel.dk)** • Astro 7 · TypeScript · Tailwind CSS v4 · Deployed on Vercel

## Features

- 📝 Type-safe content collections (notes, projects) with Markdown and Zod schemas
- 🏷️ Tag pages generated automatically with clean slug URLs
- 🎨 Light/dark/system theme with no-flash inline script and view transitions
- 🔍 SEO: JSON-LD, Open Graph, canonical URLs, RSS, sitemap
- ♿ Accessibility: skip link, focus rings, reduced-motion support, no-JS fallback
- 🔒 Security headers (CSP, HSTS, etc.) via `vercel.json`

## Development

```bash
pnpm install        # Install dependencies
pnpm dev            # Start dev server (pnpm dev:network to expose on LAN)
pnpm build          # astro check + production build to dist/
pnpm preview        # Serve the production build locally
pnpm lint           # ESLint (pnpm lint:fix to autofix)
pnpm format         # Prettier (pnpm format:check to verify)
```

**Requirements**: Node.js 22.12+, pnpm 9+

## Project Structure

```
src/
├── components/     # Reusable Astro components
├── content/        # Markdown content (notes, projects)
├── layouts/        # Page and article layouts
├── lib/            # Collection helpers and utilities
├── pages/          # File-based routing
└── styles/         # Global CSS and Tailwind config
```

## Adding Content

Create a kebab-case folder with an `index.md` in `src/content/notes/` or `src/content/projects/`:

```markdown
---
title: "Post Title"
description: "Brief description"
date: 2025-01-15
updatedDate: 2025-02-01 # optional
draft: false # optional, excludes from builds and feeds
tags: ["fp", "dotnet"] # optional, max 3
---

Your content here...
```

Projects additionally support `demoURL` and `repoURL`. Tag pages and their URLs are derived automatically from frontmatter tags.

## License

Code is [MIT](LICENSE.md), based on [astro-nano](https://github.com/markhorn-dev/astro-nano) by Mark Horn, with modifications by Daniel Larsen. Site content (posts and images) © Daniel Larsen.
