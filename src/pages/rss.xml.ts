import rss from "@astrojs/rss";
import type { APIRoute } from "astro";
import { SITE, HOME } from "@consts";
import { getAllNotes, getAllProjects } from "@lib/collections";

export const GET: APIRoute = async (context) => {
  const notes = await getAllNotes();
  const projects = await getAllProjects();

  const items = [...notes, ...projects].sort((a, b) => b.data.date.valueOf() - a.data.date.valueOf());

  return rss({
    title: SITE.NAME,
    description: HOME.DESCRIPTION,
    site: context.site ?? "https://itsdaniel.dk/",
    items: items.map((item) => ({
      title: item.data.title,
      description: item.data.description,
      pubDate: item.data.date,
      link: `/${item.collection}/${item.id}/`,
    })),
  });
};
