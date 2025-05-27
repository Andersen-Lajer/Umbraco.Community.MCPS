using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.MCPS.Helpers;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Services;
using Umbraco.Community.MCPS.Services.McpsPropagationSettingService;

namespace Umbraco.Community.MCPS.NotificationHandlers;

public class McpsContentSavingNotification(IContentPropagationService contentPropagationService, IMcpsPropagationSettingService settingService) : INotificationHandler<ContentSavingNotification>
{
    public void Handle(ContentSavingNotification notification)
    {
        foreach (var savedEntity in notification.SavedEntities)
        {
            var propagationSettings = settingService.GetAll();
            //NOTE: We should check the savedEntity for Properties with a matching Key to the PropertyTypeKey saved on setting. 
            //Setting should also be updated to use PropertyTypeKey rather than alias, as this allows for flexible backoffice naming. 

            if (propagationSettings is not null)
            {
                contentPropagationService.PropagateSavedContent(savedEntity, propagationSettings);
            }
        }
    }
}
