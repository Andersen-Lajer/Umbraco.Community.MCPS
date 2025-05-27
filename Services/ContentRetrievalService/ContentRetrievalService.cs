using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Community.MCPS.Repositories;

namespace Umbraco.Community.MCPS.Services;

public class ContentRetrievalService(IMcpsDatabaseRepository mcpsDatabaseRepository, UmbracoHelper umbracoHelper) : IContentRetrievalService
{
    public IPublishedContent? GetPropagatedContent(string id)
    {
        try
        {
            var targetId = mcpsDatabaseRepository.GetTargetGuid(id);
            var content = umbracoHelper.Content(targetId);
            return content ?? null;
        }
        catch
        {
            return null;
        }
    }
}