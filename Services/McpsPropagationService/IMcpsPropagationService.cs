using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services;

public interface IMcpsPropagationService
{
    public List<Guid> GetContentReferences(int skip, int take, string value, PropagationSetting propagationSetting, string propertyTypeAlias);
}
