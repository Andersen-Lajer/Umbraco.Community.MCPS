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
            Console.WriteLine("Started comp service");

            var container = contentTypeService.GetContainers("McpsCompositions", 1)?.FirstOrDefault();

            if (container is null)
            {
                var attempt = contentTypeService.CreateContainer(-1, Guid.NewGuid(), "McpsCompositions");
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

            // NOTE: Consider what user GUID should be used here + Change to CreateAsync
            contentTypeService.CreateAsync(returnCT, userId);

            Console.WriteLine("comp service finished");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            logger.LogError(ex, "Error in CreateComposition");
            return false;
        }
    }

    public bool DeleteComposition()
    {
        throw new NotImplementedException();
    }
}
