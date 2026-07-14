import { getCollection } from "astro:content";
import type { CollectionEntry } from "astro:content";

async function getPublished<C extends "notes" | "projects">(collection: C) {
  return (await getCollection(collection))
    .filter((entry) => !entry.data.draft)
    .sort((a, b) => b.data.date.valueOf() - a.data.date.valueOf());
}

export async function getAllNotes() {
  return getPublished("notes");
}

export async function getAllProjects() {
  return getPublished("projects");
}

export async function getAllFragrances() {
  const entries = (await getCollection("fragrances")).filter((entry) => !entry.data.draft);
  const owned = entries
    .filter((entry) => !entry.data.wishlist)
    .sort((a, b) => (b.data.rating ?? 0) - (a.data.rating ?? 0) || a.data.name.localeCompare(b.data.name));
  const wishlist = entries
    .filter((entry) => entry.data.wishlist)
    .sort((a, b) => a.data.house.localeCompare(b.data.house) || a.data.name.localeCompare(b.data.name));
  return { owned, wishlist };
}

export function groupNotesByYear(notes: CollectionEntry<"notes">[]) {
  return notes.reduce((acc: Record<string, CollectionEntry<"notes">[]>, note) => {
    const year = note.data.date.getFullYear().toString();
    if (!acc[year]) acc[year] = [];
    acc[year].push(note);
    return acc;
  }, {});
}

/** All tags across notes and projects with usage counts, sorted by count desc, then name. */
export async function getTagCounts() {
  const entries = [...(await getAllNotes()), ...(await getAllProjects())];
  const counts = new Map<string, number>();
  for (const entry of entries) {
    for (const tag of entry.data.tags ?? []) {
      counts.set(tag, (counts.get(tag) ?? 0) + 1);
    }
  }
  return [...counts.entries()].sort((a, b) => b[1] - a[1] || a[0].localeCompare(b[0]));
}
