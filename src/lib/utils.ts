import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

/** URL-safe slug for a display tag: "F#" -> "fsharp", "effect system" -> "effect-system". */
export function slugifyTag(tag: string) {
  return tag
    .toLowerCase()
    .replace(/#/g, "sharp")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
}

export function readingTime(content: string) {
  const textOnly = content
    .replace(/```[\s\S]*?```/g, "") // remove fenced code blocks (must run before backtick strip)
    .replace(/!\[[^\]]*\]\([^)]*\)/g, "") // remove images
    .replace(/\[[^\]]*\]\([^)]*\)/g, (match) => match.replace(/\[([^\]]*)\]\([^)]*\)/, "$1")) // keep link text only
    .replace(/<[^>]+>/g, "") // remove HTML tags
    .replace(/&[#a-z0-9]+;/gi, " ") // remove HTML entities
    .replace(/[*_~`#>|\\-]/g, "") // remove Markdown formatting chars
    .replace(/\n{2,}/g, " "); // collapse whitespace

  const wordCount = textOnly
    .trim()
    .split(/\s+/)
    .filter((word) => word.length > 0).length;

  const minutes = Math.ceil(wordCount / 200);

  if (minutes === 0) {
    return "< 1 min read";
  }

  return minutes === 1 ? "1 min read" : `${minutes} min read`;
}
