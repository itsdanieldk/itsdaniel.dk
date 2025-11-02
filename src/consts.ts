import type { Site, Metadata, Socials } from "@types";

export const SITE: Site = {
  NAME: "itsdaniel",
  EMAIL: "hey@itsdaniel.dk",
  NUM_NOTES_ON_HOMEPAGE: 3,
  NUM_PROJECTS_ON_HOMEPAGE: 3,
};

export const HOME: Metadata = {
  TITLE: "Home",
  DESCRIPTION: "Daniel Larsen's personal website about software engineering.",
};

export const NOTES: Metadata = {
  TITLE: "Notes",
  DESCRIPTION: "A collection of notes on topics I am passionate about.",
};

export const PROJECTS: Metadata = {
  TITLE: "Projects",
  DESCRIPTION: "A collection of my projects, with links to repositories and demos.",
};

export const ABOUT: Metadata = {
  TITLE: "About",
  DESCRIPTION: "Learn more about me and my journey.",
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
