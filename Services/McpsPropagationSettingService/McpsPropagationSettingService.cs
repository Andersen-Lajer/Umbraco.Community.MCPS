using Microsoft.Extensions.Logging;
using Umbraco.Community.MCPS.Mappers;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Repositories;

namespace Umbraco.Community.MCPS.Services;

public class McpsPropagationSettingService(
    IMcpsDatabaseRepository repository,
    ILogger<IMcpsPropagationSettingService> logger) : IMcpsPropagationSettingService
{
    private readonly McpsServiceModelMapper serviceModelMapper = new(repository);

    public PropagationSetting CreatePropagationSetting(PropagationSetting propagationSetting)
    {
        try
        {
            return serviceModelMapper.MapToServiceModel(repository.CreatePropagationSetting(propagationSetting));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CreatePropagationSetting");
            throw;
        }
    }

    public PropagationSetting UpdatePropagationSetting(PropagationSetting propagationSetting)
    {
        throw new NotImplementedException();
    }

    public bool DeletePropagationSetting(PropagationSetting propagationSetting)
    {
        throw new NotImplementedException();
    }

    public List<PropagationSetting> GetPropagationSettingsByDocumentType(string documentType)
    {
        List<PropagationSetting> propagationSettings = [];
        var propagationSettingSchemas = repository.GetPropagationSettingsByDocumentType(documentType);
        propagationSettings.AddRange(propagationSettingSchemas.Select(serviceModelMapper.MapToServiceModel));

        return propagationSettings;
    }

    public List<PropagationSetting> GetAll()
    {
        List<PropagationSetting> propagationSettings = [];
        var propagationSettingSchemas = repository.GetAllPropagationSettings();
        propagationSettings.AddRange(propagationSettingSchemas.Select(serviceModelMapper.MapToServiceModel));

        return propagationSettings;
    }
}