import type { Site, Metadata, Socials } from "@types";

export const SITE: Site = {
  NAME: "itsdaniel",
  EMAIL: "hey@itsdaniel.dk",
  NUM_NOTES_ON_HOMEPAGE: 3,
  NUM_PROJECTS_ON_HOMEPAGE: 3,
};

export const HOME: Metadata = {
  TITLE: "Home",
  DESCRIPTION: "Software engineer writing about functional programming, distributed systems, and .NET. By Daniel Larsen.",
};

export const NOTES: Metadata = {
  TITLE: "Notes",
  DESCRIPTION: "Notes on functional programming, software architecture, and lessons from building backend systems.",
};

export const PROJECTS: Metadata = {
  TITLE: "Projects",
  DESCRIPTION: "Open source projects and experiments in F#, .NET, and distributed systems.",
};

export const ABOUT: Metadata = {
  TITLE: "About",
  DESCRIPTION: "About Daniel Larsen, a software engineer from Denmark specializing in functional programming and cloud-native .NET systems.",
};

export const SOCIALS: Socials = [
  {
    NAME: "github",
    HREF: "https://github.com/itsdanieldk/",
  },
  {
    NAME: "linkedin",
    HREF: "https://www.linkedin.com/in/itsdanieldk/",
  },
];
