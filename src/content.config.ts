import { defineCollection } from "astro:content";
import { glob } from "astro/loaders";
import { z } from "astro/zod";

const baseSchema = z.object({
  title: z.string(),
  description: z.string(),
  date: z.coerce.date(),
  updatedDate: z.coerce.date().optional(),
  draft: z.boolean().optional(),
  tags: z.array(z.string()).max(3).optional(),
});

const notes = defineCollection({
  loader: glob({ base: "./src/content/notes", pattern: "**/*.md" }),
  schema: baseSchema,
});

const projects = defineCollection({
  loader: glob({ base: "./src/content/projects", pattern: "**/*.md" }),
  schema: baseSchema.extend({
    demoURL: z.string().optional(),
    repoURL: z.string().optional(),
  }),
});

const fragrances = defineCollection({
  loader: glob({ base: "./src/content/fragrances", pattern: "**/*.yaml" }),
  schema: ({ image }) =>
    z
      .object({
        name: z.string(),
        house: z.string(),
        url: z.url(),
        rating: z.number().min(1).max(10).multipleOf(0.5).optional(),
        note: z.string().optional(),
        concentration: z.string().optional(),
        image: image(),
        wishlist: z.boolean().optional(),
        draft: z.boolean().optional(),
      })
      .superRefine((data, ctx) => {
        if (data.wishlist && data.rating !== undefined) {
          ctx.addIssue({ code: z.ZodIssueCode.custom, message: "wishlist entries must not have a rating", path: ["rating"] });
        }
        if (!data.wishlist && data.rating === undefined) {
          ctx.addIssue({ code: z.ZodIssueCode.custom, message: "rating is required unless wishlist is true", path: ["rating"] });
        }
      }),
});

export const collections = { notes, projects, fragrances };
