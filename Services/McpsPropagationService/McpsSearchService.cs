using Examine;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Community.MCPS.Helpers;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Enums;
using Umbraco.Extensions;

namespace Umbraco.Community.MCPS.Services;

class McpsSearchService(IExamineManager examineManager, IPublishedContentQuery publishedContentQuery) : IMcpsPropagationService
{
    private IEnumerable<PublishedSearchResult> SearchForValueInContentWithPropertyType(string contentTypeAlias, string propertyTypeAlias, string searchTerm, int skip, int take)
    {
        if (!examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
        {
            throw new InvalidOperationException($"No index found by name{Constants.UmbracoIndexes.ExternalIndexName}");
        }
        var query = index.Searcher.CreateQuery(IndexTypes.Content);
        var queryExecutor = query.NodeTypeAlias(contentTypeAlias).And().Field(propertyTypeAlias, searchTerm);

        foreach (var result in publishedContentQuery.Search(queryExecutor, skip, take, out var _))
        {
            yield return result;
        }
    }

    public List<Guid> GetContentReferences(int skip, int take, string value, PropagationSetting propagationSetting, string propertyTypeAlias)
    {
        List<Guid> guids = [];
        List<IPublishedContent> content = [];

        foreach (var contentTypeAlias in propagationSetting.ContentTypes)
        {
            var searchResults = SearchForValueInContentWithPropertyType(contentTypeAlias, propertyTypeAlias, value, skip, take);
            content.AddRange(searchResults.Select(x => x.Content));
        }
        // NOTE: Proper priority handling should be implemented in the future, for now we use a default priority order.
        if (propagationSetting.PropagationPriority.Count != 3)
        {
            propagationSetting.PropagationPriority = [PropagationRank.Relevant, PropagationRank.Newest, PropagationRank.MostPopular];
        }
        content = McpsPublishedContentHelper.Sort(content, propagationSetting.PropagationPriority);
        guids.AddRange(content.Select(x => x.Key));

        return guids;
    }
}