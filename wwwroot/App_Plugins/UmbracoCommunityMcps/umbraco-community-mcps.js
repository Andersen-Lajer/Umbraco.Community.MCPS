const a = [
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
    js: () => import("./mcps-manage-dashboard.element-DljB8y5V.js"),
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
], n = [
  ...a
];
export {
  n as manifests
};
//# sourceMappingURL=umbraco-community-mcps.js.map
