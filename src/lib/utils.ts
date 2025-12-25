import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatDate(date: Date) {
  return Intl.DateTimeFormat("en-US", {
    month: "short",
    day: "2-digit",
    year: "numeric",
  }).format(date);
}

export function readingTime(html: string) {
  const textOnly = html.replace(/<[^>]+>/g, "").replace(/&[#a-z0-9]+;/gi, " ");

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
