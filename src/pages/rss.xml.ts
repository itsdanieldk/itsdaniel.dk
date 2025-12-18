import rss from "@astrojs/rss";
import { HOME } from "@consts";
import { getAllNotes, getAllProjects } from "@lib/collections";

type Context = {
  site: string;
};

export async function GET(context: Context) {
  const notes = await getAllNotes();
  const projects = await getAllProjects();

  const items = [...notes, ...projects].sort((a, b) => b.data.date.valueOf() - a.data.date.valueOf());

  return rss({
    title: HOME.TITLE,
    description: HOME.DESCRIPTION,
    site: context.site,
    items: items.map((item) => ({
      title: item.data.title,
      description: item.data.description,
      pubDate: item.data.date,
      link: `/${item.collection}/${item.slug}/`,
    })),
  });
}
