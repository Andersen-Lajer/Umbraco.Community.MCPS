using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Community.MCPS.Models.Enums;

namespace Umbraco.Community.MCPS.Helpers
{
    public static class McpsPublishedContentHelper
    {
        public static List<IPublishedContent> Sort(List<IPublishedContent> content, List<PropagationRank> propagationPriority)
        {
            return propagationPriority switch
            {
                // NOTE : A bit hacky, this should probably be refactored to use a more generic approach
                [PropagationRank.Relevant, PropagationRank.Newest, PropagationRank.MostPopular] => [.. content.OrderByDescending(x => x.UpdateDate)], //NOTE : This is a placeholder for the actual implementation of relevance sorting.
                [PropagationRank.Relevant, PropagationRank.MostPopular, PropagationRank.Newest] => [.. content.OrderByDescending(x => x.UpdateDate)],
                [PropagationRank.Newest, PropagationRank.Relevant, PropagationRank.MostPopular] => throw new NotImplementedException(),
                [PropagationRank.Newest, PropagationRank.MostPopular, PropagationRank.Relevant] => throw new NotImplementedException(),
                [PropagationRank.MostPopular, PropagationRank.Relevant, PropagationRank.Newest] => throw new NotImplementedException(),
                [PropagationRank.MostPopular, PropagationRank.Newest, PropagationRank.Relevant] => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(List<PropagationRank>), propagationPriority, null),
            };
        }
    }
}