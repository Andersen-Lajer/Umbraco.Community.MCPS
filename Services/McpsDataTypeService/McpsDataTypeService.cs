using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.MCPS.NotificationHandlers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Umbraco.Cms.Core.Media.EmbedProviders;


namespace Umbraco.Community.MCPS.Services;


class McpsDataTypeService(
    IDataTypeService dataTypeService,
    IConfigurationEditorJsonSerializer jsonEditor,
    ILogger<McpsUmbracoStartingNotification> _logger,
    IContentTypeService _contentTypeService,
    IShortStringHelper _shortStringHelper
    ) : IMcpsDataTypeService
{
    public async Task<IDataType> CreateMcpsDataType(string dataTypeName, Dictionary<string, object> configurationData, Guid userId)
    {
        var mcpsDatatypeName = $"Mcps {dataTypeName}";

        var mcpsDataType = await dataTypeService.GetAsync(mcpsDatatypeName);
        if (mcpsDataType is not null)
        {
            _logger.LogInformation("McpsDataType already exists");
            return mcpsDataType;
        }

        var dropdownDataType = await dataTypeService.GetAsync(dataTypeName) ?? throw new Exception("Error in CreateMcpsDataType");
        DataType dt = new(dropdownDataType.Editor, jsonEditor)
        {
            EditorUiAlias = dropdownDataType.EditorUiAlias,
            CreateDate = DateTime.Now,
            ConfigurationData = configurationData,
            Name = mcpsDatatypeName
        };

        var createdDt = await dataTypeService.CreateAsync(dt, userId);
        var result = createdDt.Result;
        return result;
    }


    public async Task<ContentType> CreateMcpsContentType(Guid userId, PropertyType? propertyType, string name, string alias, string containerName, List<IContentTypeComposition>? compositions)
    {

        PropertyGroup propertyGroup = new(false) { Alias = "content", Name = "Content", Type = PropertyGroupType.Tab };
        if (propertyGroup is null)
        {
            _logger.LogError("Failed to create PropertyGroup");
            throw new Exception("Error in CreateMcpsContentType");
        }

        if (propertyType is not null)
        {
            propertyGroup.PropertyTypes?.Add(propertyType);
        }

        ContentType contentType = new(_shortStringHelper, -1)
        {
            Alias = alias,
            Icon = "icon-clothes-hanger color-deep-purple",
            Name = name,
            IsElement = true
        };
        contentType.PropertyGroups.Add(propertyGroup);
        if (compositions is not null)
        {
            foreach (var composition in compositions)
            {
                contentType.AddContentType(composition);
            }
        }
        var container = _contentTypeService.GetContainers(containerName, 1)?.FirstOrDefault();

        if (container is null)
        {
            var attempt = _contentTypeService.CreateContainer(-1, Guid.NewGuid(), containerName);

            container = attempt.Result?.Entity;
            if (container is not null)
            {
                _contentTypeService.SaveContainer(container);
            }
        }
        if (container is not null)
        {
            contentType.ParentId = container.Id;
        }
        var existingComposition = _contentTypeService.Get(contentType.Alias);

        if (existingComposition is null)
        {
            await _contentTypeService.CreateAsync(contentType, userId);
        }

        return contentType;
    }
}
