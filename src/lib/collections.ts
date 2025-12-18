import { getCollection } from "astro:content";
import type { CollectionEntry } from "astro:content";

export async function getAllNotes() {
  return (await getCollection("notes")).filter((note) => !note.data.draft).sort((a, b) => b.data.date.valueOf() - a.data.date.valueOf());
}

export async function getAllProjects() {
  return (await getCollection("projects"))
    .filter((project) => !project.data.draft)
    .sort((a, b) => b.data.date.valueOf() - a.data.date.valueOf());
}

export function groupNotesByYear(notes: CollectionEntry<"notes">[]) {
  return notes.reduce((acc: Record<string, CollectionEntry<"notes">[]>, note) => {
    const year = note.data.date.getFullYear().toString();
    if (!acc[year]) acc[year] = [];
    acc[year].push(note);
    return acc;
  }, {});
}
