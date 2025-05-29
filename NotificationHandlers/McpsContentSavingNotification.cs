using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.MCPS.Services;

namespace Umbraco.Community.MCPS.NotificationHandlers;

public class McpsContentSavingNotification(IContentPropagationService contentPropagationService, IMcpsPropagationSettingService settingService) : INotificationHandler<ContentSavingNotification>
{
    public void Handle(ContentSavingNotification notification)
    {
        foreach (var savedEntity in notification.SavedEntities)
        {
            var propagationSettings = settingService.GetAll();

            if (propagationSettings is not null)
            {
                contentPropagationService.PropagateSavedContent(savedEntity, propagationSettings);
            }
        }
    }
}