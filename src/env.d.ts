/// <reference path="../.astro/types.d.ts" />
/// <reference types="astro/client" />

interface Window {
  __theme?: {
    toggle: (dark: boolean) => void;
  };
}
