using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Services.McpsRelationService;

public interface IMcpsRelationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="pageSettings"></param>
    /// <returns>Returns a count of relations on the page</returns>
    public int UpdateContentRelations(IContent content, List<PropagationSetting> pageSettings);

    //NOTE: Probably private
    public bool UpdateRelation(int positionId, PropagationSetting pageSetting);

    //NOTE: Probably private
    public bool DeleteRelation(int relationId);
}
