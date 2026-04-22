# 🪻 itsdaniel.dk

Personal blog and portfolio focused on software engineering, functional programming, and distributed systems.

**[itsdaniel.dk](https://itsdaniel.dk)** • Built with Astro, TypeScript, and Tailwind CSS

## Features

- 📝 Type-safe content collections with Markdown/MDX
- 🎨 Dark/light theme with system preference support
- ⚡ Static site generation for optimal performance
- 🔍 SEO optimized with JSON-LD, Open Graph, RSS
- ♿ WCAG compliant accessibility

## Development

```bash
# Install dependencies
pnpm install

# Start dev server
pnpm dev

# Build for production
pnpm build
```

**Requirements**: Node.js 22.12.0+, pnpm 9+

## Project Structure

```
src/
├── components/     # Reusable Astro components
├── content/        # Markdown content (notes, projects)
├── layouts/        # Page layouts
├── pages/          # File-based routing
└── styles/         # Global CSS and Tailwind config
```

## Adding Content

Create a new Markdown file in `src/content/notes/` or `src/content/projects/`:

```markdown
---
title: "Post Title"
description: "Brief description"
date: 2025-01-15
draft: false
---

Your content here...
```

## License

MIT - Mark Horn
