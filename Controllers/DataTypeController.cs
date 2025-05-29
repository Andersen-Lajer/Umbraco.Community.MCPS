using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.MCPS.Services;
using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Controllers;

[Route("mcps/api/rules")]
public class DataTypeApiController(
    IDataTypeService dataTypeService,
    IShortStringHelper shortStringHelper,
    IMcpsDataTypeService mcpsDataTypeService,
    IUserService userService,
    IContentTypeService contentTypeService,
    IMcpsPropagationSettingService propagationSettingService,
    ILogger<DataTypeApiController> logger,
    IWebHostEnvironment env)
{
    // NOTE: This should be refactored. 
    // Currently it contains at minimum three responsibilities: creating a data type, creating a content type, and creating a file.
    // These can be separated into different methods or services for better maintainability and readability.
    [HttpPost("create/{userEmail}")]
    public async void CreateRuleDatatype([FromRoute] string userEmail, [FromBody] PropagationSetting propagationSetting)
    {
        // Get required context and variables. 
        var dt = await dataTypeService.GetAsync(propagationSetting.PropertyAlias);
        if (dt is null || dt.Name is null) { return; }

        var propertyAlias = char.ToLower(dt.Name[0]) + dt.Name[1..].Replace(" ", "");

        propagationSetting.PropertyAlias = propertyAlias;
        var user = userService.GetByEmail(userEmail);
        var userId = Guid.NewGuid();
        if (user is not null)
        {
            userId = user.Key;
        }
        var docGuids = propagationSetting.ContentTypes.Select(x => new Guid(x));
        var docTypes = contentTypeService.GetMany(docGuids);
        if (docTypes is null || !docTypes.Any()) { return; }

        propagationSetting.ContentTypes = [.. docTypes.Select(x => x.Alias)];

        if (string.IsNullOrWhiteSpace(propagationSetting.PropertyAlias)) { return; }

        var stringLabel = await dataTypeService.GetAsync("Label (string)");
        if (stringLabel is null) { return; }

        var compositionNames = GenerateName(propagationSetting.PropertyAlias, propagationSetting.ContentTypes, "Composition");
        var blockNames = GenerateName(propagationSetting.PropertyAlias, propagationSetting.ContentTypes, "Block");

        var existingContent = contentTypeService.GetAll().Any(x => x.Name == compositionNames.name);
        if (existingContent) { return; }

        var dataPropertyType = new PropertyType(shortStringHelper, dt) { Alias = compositionNames.propertyAlias, Name = compositionNames.name };
        var contentComposition = await mcpsDataTypeService.CreateMcpsContentType(userId, dataPropertyType, compositionNames.name, compositionNames.contentAlias, compositionNames.containerName, null);
        if (contentComposition is not null)
        {
            List<IContentTypeComposition> compositions = [contentComposition];
            var referenceIdComposition = contentTypeService.Get(McpsConstants.CamelPrefix + McpsConstants.ReferenceIdAlias);
            if (referenceIdComposition is not null)
            {
                compositions.Add(referenceIdComposition);
            }

            var block = await mcpsDataTypeService.CreateMcpsContentType(userId, null, blockNames.name, blockNames.contentAlias, blockNames.containerName, compositions);

            // Create PartialView for the new block.
            var blocksDirectory = Path.Combine(env.ContentRootPath, "Views", "Partials", "McpsBlocks");
            Directory.CreateDirectory(blocksDirectory);

            var filePath = Path.Combine(blocksDirectory, block.Alias + ".cshtml");
            try
            {
                using FileStream fs = System.IO.File.Create(filePath);
                byte[] info = new UTF8Encoding(true).GetBytes("@using Umbraco.Cms.Core.Models;\r\n@using Umbraco.Extensions\r\n@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<Umbraco.Cms.Core.Models.Blocks.BlockGridItem>\r\n@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;\r\n@using Umbraco.Community.MCPS.Services;\r\n@inject IContentRetrievalService ContentRetrievalService;\r\n\r\n@{\r\n var blockId = Model.Content.Value(\"referenceId\") as string;\r\n    if (blockId is null) { return; }\r\n    var propagatedContent = ContentRetrievalService.GetPropagatedContent(blockId);\r\n    if (propagatedContent is null) { return; }\r\n}\r\n\r\n <section>\r\n <div>\r\n\t\t @propagatedContent.Name\r\n </div>\r\n </section> ");
                fs.Write(info, 0, info.Length);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create file");
            }

            // Register the ContentType with Umbraco.
            foreach (var contentType in propagationSetting.ContentTypes)
            {
                var contentTypeToUpdate = contentTypeService.Get(contentType);
                if (contentTypeToUpdate is not null)
                {
                    if (contentTypeToUpdate.CompositionAliases().Contains(contentComposition.Alias))
                    {
                        logger.LogInformation($"Content type {contentType} already contains the composition {contentComposition.Name}.");
                        continue;
                    }
                    contentTypeToUpdate.AddContentType(contentComposition);
                    await contentTypeService.UpdateAsync(contentTypeToUpdate, userId);
                }
            }
            propagationSetting.PropertyAlias = compositionNames.propertyAlias;
            var setting = propagationSettingService.CreatePropagationSetting(propagationSetting);
        }
    }

    private static (string name, string contentAlias, string propertyAlias, string containerName) GenerateName(string editorAlias, List<string> documentTypes, string elementType)
    {
        var upperCaseDocumentTypes = documentTypes.Select(x => char.ToUpper(x[0]) + x[1..]).ToList();
        string name = "[" + editorAlias + "] - " + elementType + " - [" + string.Join(" | ", upperCaseDocumentTypes) + "]";
        string contentAlias = editorAlias + elementType + string.Join("", upperCaseDocumentTypes);
        string propertyAlias = editorAlias + string.Join("", upperCaseDocumentTypes);
        string containerName = McpsConstants.PascalPrefix + elementType + "s";

        return (name, contentAlias, propertyAlias, containerName);
    }
}