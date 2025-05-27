using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Community.MCPS.Mappers;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Repositories;

namespace Umbraco.Community.MCPS.Services.McpsPropagationSettingService;

public class McpsPropagationSettingService(
    IMcpsDatabaseRepository _repository,
    ILogger<IMcpsPropagationSettingService> _logger) : IMcpsPropagationSettingService
{

    private readonly McpsServiceModelMapper serviceModelMapper = new(_repository);

    public PropagationSetting CreatePropagationSetting(PropagationSetting propagationSetting)
    {
        try
        {
            return serviceModelMapper.MapToServiceModel(_repository.CreatePropagationSetting(propagationSetting));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
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

        var propagationSettingSchemas = _repository.GetPropagationSettingsByDocumentType(documentType);

        propagationSettings.AddRange(propagationSettingSchemas.Select(serviceModelMapper.MapToServiceModel));

        return propagationSettings;
    }

    public List<PropagationSetting> GetAll()
    {
        List<PropagationSetting> propagationSettings = [];

        var propagationSettingSchemas = _repository.GetAllPropagationSettings();

        propagationSettings.AddRange(propagationSettingSchemas.Select(serviceModelMapper.MapToServiceModel));

        return propagationSettings;
    }

}
