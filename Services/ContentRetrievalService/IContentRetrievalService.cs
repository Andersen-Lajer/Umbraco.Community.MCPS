using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.MCPS.Services;

public interface IContentRetrievalService
{
    public IPublishedContent? GetPropagatedContent(string targetId);
}
