using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services;

public interface IMcpsRelationService
{
    int UpdateContentRelations(IContent content, List<PropagationSetting> pageSettings);
}