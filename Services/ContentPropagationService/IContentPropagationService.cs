using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services;

public interface IContentPropagationService
{
    public bool PropagateSavedContent(IContent savedEntity, List<PropagationSetting> pageSettings);

    public bool CreatePropagationSetting(PropagationSetting setting);
}
