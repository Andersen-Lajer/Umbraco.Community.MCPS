const a = [
  {
    name: "Umbraco Community Mcps Entrypoint",
    alias: "Umbraco.Community.Mcps.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-CQs6WTUY.js")
  }
], o = [
  {
    name: "Umbraco Community Mcps Dashboard",
    alias: "Umbraco.Community.Mcps.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element-DS5Mdwvi.js"),
    meta: {
      label: "Example Dashboard",
      pathname: "example-dashboard"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content"
      }
    ]
  },
  {
    type: "section",
    alias: "Mcps.Section",
    name: "MCPS Section",
    meta: {
      label: "MCPS",
      pathname: "mcps"
    }
  },
  {
    name: "Umbraco Community Mcps Manage Dashboard",
    alias: "Umbraco.Community.Mcps.Manage.Dashboard",
    type: "dashboard",
    js: () => import("./mcps-manage-dashboard.element-n4md31BC.js"),
    meta: {
      label: "Manage Dashboard",
      pathname: "manage-dashboard"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Mcps.Section"
      }
    ]
  }
], t = [
  ...a,
  ...o
];
export {
  t as manifests
};
//# sourceMappingURL=umbraco-community-mcps.js.map
