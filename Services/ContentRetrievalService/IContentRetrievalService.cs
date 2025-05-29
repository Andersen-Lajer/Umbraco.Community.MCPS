using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.MCPS.Services;

public interface IContentRetrievalService
{
    IPublishedContent? GetPropagatedContent(string targetId);
}