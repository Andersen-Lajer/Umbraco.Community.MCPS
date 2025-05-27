export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Community Mcps Entrypoint",
    alias: "Umbraco.Community.Mcps.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
