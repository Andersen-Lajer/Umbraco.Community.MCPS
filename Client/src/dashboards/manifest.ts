export const manifests: Array<UmbExtensionManifest> = [
    {
        "type": "section",
        "alias": "Mcps.Section",
        "name": "MCPS Section",
        "meta": {
            "label": "MCPS",
            "pathname": "mcps"
        }
    },
    {
        name: "Umbraco Community Mcps Manage Dashboard",
        alias: "Umbraco.Community.Mcps.Manage.Dashboard",
        type: "dashboard",
        js: () => import("./mcps-manage-dashboard.element.js"),
        meta: {
            label: "Manage Dashboard",
            pathname: "manage-dashboard",
        },
        conditions: [
            {
                "alias": "Umb.Condition.SectionAlias",
                "match": "Mcps.Section"
            },
        ],
    },
];
