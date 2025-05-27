using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Community.MCPS.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.MCPS.Services.McpsPropagationSettingService;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Helpers;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Hosting;


[Route("mcps/api/rules")]
public class DataTypeApiController
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IMcpsDataTypeService _mcpsDataTypeService;
    private readonly IUserService _userService;
    private readonly IContentTypeService _contentTypeService;
    private readonly IMcpsPropagationSettingService _propagationSettingService;
    private readonly ILogger<DataTypeApiController> _logger;
    private readonly IWebHostEnvironment _env;

    public DataTypeApiController(IDataTypeService dataTypeService, IShortStringHelper shortStringHelper, IMcpsDataTypeService mcpsDataTypeService, IUserService userService, IContentTypeService contentTypeService, IMcpsPropagationSettingService propagationSettingService, ILogger<DataTypeApiController> logger, IWebHostEnvironment env)
    {
        _dataTypeService = dataTypeService;
        _shortStringHelper = shortStringHelper;
        _mcpsDataTypeService = mcpsDataTypeService;
        _userService = userService;
        _contentTypeService = contentTypeService;
        _propagationSettingService = propagationSettingService;
        _logger = logger;
        _env = env;
    }

    [HttpPost("create/{userEmail}")]
    public async void CreateRuleDatatype([FromRoute] string userEmail, [FromBody] PropagationSetting propagationSetting)
    {
        var dt = await _dataTypeService.GetAsync(propagationSetting.PropertyAlias);

        if (dt is null || dt.Name is null) { return; }

        var propertyAlias = char.ToLower(dt.Name[0]) + dt.Name.Substring(1).Replace(" ", "");

        propagationSetting.PropertyAlias = propertyAlias;
        var user = _userService.GetByEmail(userEmail);
        var userId = Guid.NewGuid();
        if (user is not null)
        {
            userId = user.Key;
        }
        var docGuids = propagationSetting.ContentTypes.Select(x => new Guid(x));
        var docTypes = _contentTypeService.GetMany(docGuids);
        if (docTypes is null || docTypes.Count() == 0) { return; }
        propagationSetting.ContentTypes = docTypes.Select(x => x.Alias).ToList();

        if (string.IsNullOrWhiteSpace(propagationSetting.PropertyAlias)) { return; }

        var stringLabel = await _dataTypeService.GetAsync("Label (string)");

        if (stringLabel is null) { return; }


        var compositionNames = GenerateName(propagationSetting.PropertyAlias, propagationSetting.ContentTypes, "Composition");
        var blockNames = GenerateName(propagationSetting.PropertyAlias, propagationSetting.ContentTypes, "Block");

        var existingContent = _contentTypeService.GetAll().Any(x => x.Name == compositionNames.name);

        if (existingContent) { return; }


        var dataPropertyType = new PropertyType(_shortStringHelper, dt) { Alias = compositionNames.propertyAlias, Name = compositionNames.name };

        var contentComposition = await _mcpsDataTypeService.CreateMcpsContentType(userId, dataPropertyType, compositionNames.name, compositionNames.contentAlias, compositionNames.containerName, null);
        if (contentComposition is not null)
        {
            List<IContentTypeComposition> compositions = [contentComposition];
            var referenceIdComposition = _contentTypeService.Get(ConstStrings.CamelPrefix + ConstStrings.ReferenceIdAlias);
            if (referenceIdComposition is not null)
            {
                compositions.Add(referenceIdComposition);
            }

            var block = await _mcpsDataTypeService.CreateMcpsContentType(userId, null, blockNames.name, blockNames.contentAlias, blockNames.containerName, compositions);

            var filePath = Path.Combine(_env.ContentRootPath, "Views", "Partials", "Blocks", char.ToUpper(block.Alias[0]) + block.Alias[1..] + ".cshtml");
            try
            {
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("@using Umbraco.Cms.Core.Models;\r\n@using Umbraco.Extensions\r\n@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<Umbraco.Cms.Core.Models.Blocks.BlockGridItem>\r\n@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;\r\n@using Umbraco.Community.MCPS.Services;\r\n@inject IContentRetrievalService ContentRetrievalService;\r\n\r\n@{\r\n var blockId = Model.Content.Value(\"referenceId\") as string;\r\n    if (blockId is null) { return; }\r\n    var propagatedContent = ContentRetrievalService.GetPropagatedContent(blockId);\r\n    if (propagatedContent is null) { return; }\r\n}\r\n\r\n <section>\r\n <div>\r\n\t\t @propagatedContent.Name\r\n </div>\r\n </section> ");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create file", ex.Message);
                throw new Exception("Error in CreateMcpsContentType", ex);
            }

            foreach (var contentType in propagationSetting.ContentTypes)
            {
                var contentTypeToUpdate = _contentTypeService.Get(contentType);
                if (contentTypeToUpdate is not null)
                {
                    if (contentTypeToUpdate.CompositionAliases().Contains(contentComposition.Alias))
                    {
                        _logger.LogInformation($"Content type {contentType} already contains the composition {contentComposition.Name}.");
                        continue;
                    }
                    contentTypeToUpdate.AddContentType(contentComposition);
                    await _contentTypeService.UpdateAsync(contentTypeToUpdate, userId);
                }
            }
            propagationSetting.PropertyAlias = compositionNames.propertyAlias;
            var setting = _propagationSettingService.CreatePropagationSetting(propagationSetting);
        }
    }

    private (string name, string contentAlias, string propertyAlias, string containerName) GenerateName(string editorAlias, List<string> documentTypes, string elementType)
    {
        var upperCaseDocumentTypes = documentTypes.Select(x => char.ToUpper(x[0]) + x.Substring(1)).ToList();
        string name = "[" + editorAlias + "] - " + elementType + " - [" + string.Join(" | ", upperCaseDocumentTypes) + "]";
        string contentAlias = editorAlias + elementType + string.Join("", upperCaseDocumentTypes);
        string propertyAlias = editorAlias + string.Join("", upperCaseDocumentTypes);
        string containerName = ConstStrings.PascalPrefix + elementType + "s";

        return (name, contentAlias, propertyAlias, containerName);
    }
}
