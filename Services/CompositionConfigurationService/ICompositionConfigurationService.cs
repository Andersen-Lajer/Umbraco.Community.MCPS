using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.MCPS.Services;

public interface ICompositionConfigurationService
{
    bool CreateComposition(ContentType contentType, Guid userId);
}