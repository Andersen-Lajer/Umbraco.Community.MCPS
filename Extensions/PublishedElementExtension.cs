

using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.MCPS.Extensions;

public static class PublishedElementExtension
{
    public static T GetPropagatedContent<T>(this IPublishedElement publishedElement) where T : class
    {
        throw new NotImplementedException();
    }
}
