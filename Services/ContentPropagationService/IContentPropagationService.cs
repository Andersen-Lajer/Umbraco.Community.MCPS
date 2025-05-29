using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services;

public interface IContentPropagationService
{
    bool PropagateSavedContent(IContent savedEntity, List<PropagationSetting> pageSettings);
    bool CreatePropagationSetting(PropagationSetting setting);
}