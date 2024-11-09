export type SiteConfig = typeof siteConfig;

export const siteConfig = {
  name: "TaskTracker",
  description: "Site made by Command 3.",
  navItems: [
    {
      label: "Home",
      href: "/",
    },
  ],
  navMenuItems: [
    {
      label: "Logout",
      href: "/logout",
    },
  ],
  links: {
    github: "https://github.com/sergehi/C--2024-05-Team3",
  },
};
