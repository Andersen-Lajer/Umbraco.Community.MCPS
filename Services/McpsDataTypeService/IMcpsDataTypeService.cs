using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.MCPS.Services;

public interface IMcpsDataTypeService
{
    Task<IDataType> CreateMcpsDataType(string? dataTypePrefix, string dataTypeName, Dictionary<string, object> configurationData, Guid userId);
    Task<ContentType> CreateMcpsContentType(Guid userId, PropertyType? propertyType, string name, string alias, string containerName, List<IContentTypeComposition>? compositions);
}