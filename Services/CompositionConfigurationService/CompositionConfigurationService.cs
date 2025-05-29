using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.MCPS.NotificationHandlers;

namespace Umbraco.Community.MCPS.Services;

public class CompositionConfigurationService(IContentTypeService contentTypeService, ILogger<McpsUmbracoStartingNotification> logger) : ICompositionConfigurationService
{
    public bool CreateComposition(ContentType contentType, Guid userId)
    {
        try
        {
            logger.LogInformation("Started comp service");

            var container = contentTypeService.GetContainers("McpsCompositions", 1)?.FirstOrDefault();

            if (container is null)
            {
                var attempt = contentTypeService.CreateContainer(-1, Guid.NewGuid(), "McpsCompositions");
                if (attempt.Result is null || attempt.Result.Entity is null)
                {
                    logger.LogError("Failed to create container for compositions");
                    return false;
                }
                container = attempt.Result.Entity;
                contentTypeService.SaveContainer(container);
            }

            contentType.ParentId = container.Id;
            ContentType returnCT;
            var tmpContentType = contentTypeService.Get(contentType.Alias) as ContentType;
            if (tmpContentType is not null)
            {
                tmpContentType.PropertyGroups = contentType.PropertyGroups;
                returnCT = tmpContentType;
            }
            else
            {
                returnCT = contentType;
            }
            contentTypeService.CreateAsync(returnCT, userId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CreateComposition");
            return false;
        }
    }
}