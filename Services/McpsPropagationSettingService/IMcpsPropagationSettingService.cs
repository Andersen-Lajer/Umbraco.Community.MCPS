using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services;

public interface IMcpsPropagationSettingService
{
    PropagationSetting CreatePropagationSetting(PropagationSetting propagationSetting);
    PropagationSetting UpdatePropagationSetting(PropagationSetting propagationSetting);
    bool DeletePropagationSetting(PropagationSetting propagationSetting);
    List<PropagationSetting> GetPropagationSettingsByDocumentType(string documentType);
    List<PropagationSetting> GetAll();
}