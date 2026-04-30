/// <reference path="../.astro/types.d.ts" />
/// <reference types="astro/client" />

declare module "@fontsource-variable/fira-code";

interface Window {
  __theme?: {
    toggle: (dark: boolean) => void;
  };
}
