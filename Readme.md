# Umbraco.Community.MCPS

Umbraco.Community.MCPS is an extension for Umbraco CMS that provides a seamless way to propagate content based on customizable rules.
The extension aims to provide developers with a tool to empower content editors to create dynamic sites with minimal effort.

---
## Extension usage
1. Install the Umbraco.Community.MCPS package. Either from this repository or with the latest nuget found in /bin/debug/
2. Run your Umbraco instance and log in to the backoffice. 
   - If you installed the package from this repository, you may need to run `dotnet build` in the root of the Umbraco.Community.MCPS project to generate the necessary files.
   - If you installed the package from NuGet, it should already be available in your Umbraco instance.
3. Add the `MCPS` section to the relevant user groups in the Umbraco backoffice.
4. Navigate to the `MCPS` section in the Umbraco backoffice.
5. Create a new propagation rule based on your requirements.
6. Navigate to the `Settings` section in the Umbraco backoffice.
7. You should now see a new Block in the `McpsBlock` folder.
   - This block can be used to create dynamically propagated PartialViews.
   - Should you require additional properties, you can add them to the `McpsBlock` document type.
   - Properties on generated Compositions should not be removed or edited, as they are used by the extension to function correctly.
   - Feel free to add additional properties to the Compositions if you understand the implications of doing so.
8. Your selected PropertyType will now be available on any content you selected in the propagation rule.
   - The PropertyType will be available in the `Content` section of the Umbraco backoffice.
   - You can use this PropertyType to create dynamic relationships between content and the propagated blocks. 
9. A PartialView will have been generated for your propagation block in the `Views/Partials/McpsBlocks/` folder.
   - This partial can be used to render the propagated blocks in your templates.
   - Feel free to edit the PartialView to suit your needs.
   - The partial contains some basic suggestions on how to use the propagated blocks, but you can customize it further as needed.


---

## Requirements

- Node LTS Version 20.17.0+
- Use a tool such as NVM (Node Version Manager) for your OS to help manage multiple versions of Node

## Node Version Manager Tools

- [nvm-windows](https://github.com/coreybutler/nvm-windows)
- [nvm-sh](https://github.com/nvm-sh/nvm)
- [Volta](https://docs.volta.sh/guide/getting-started)

## Setup Steps

1. Open a terminal inside the `Client` folder.
2. Run `npm install` to install all dependencies.
3. Run `npm run build` to build the project.
4. The build output is copied to `wwwroot\App_Plugins\UmbracoCommunityMcps\umbraco-community-mcps.js`.

## File Watching

- Add this Razor Class Library Project as a project reference to an Umbraco Website project.
- From the `Client` folder, run `npm run watch` to monitor changes to `.ts` files and rebuild the project automatically.
- With the Umbraco website project running, the Razor Class Library Project will refresh the browser when the build is complete.

## Suggestion

- Use VSCode as the editor of choice for its strong TypeScript tooling and recommended Lit WebComponent extensions.

## Other Resources

- [Umbraco Docs – Customizing Overview](https://docs.umbraco.com/umbraco-cms/customizing/overview)