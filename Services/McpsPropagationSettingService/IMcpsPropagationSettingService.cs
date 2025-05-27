using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services.McpsPropagationSettingService;

public interface IMcpsPropagationSettingService
{
    public PropagationSetting CreatePropagationSetting(PropagationSetting propagationSetting);
    public PropagationSetting UpdatePropagationSetting(PropagationSetting propagationSetting);
    public bool DeletePropagationSetting(PropagationSetting propagationSetting);
    public List<PropagationSetting> GetPropagationSettingsByDocumentType(string documentType);
    public List<PropagationSetting> GetAll();

}
