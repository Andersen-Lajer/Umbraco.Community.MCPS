using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Community.MCPS.Services;

namespace Umbraco.Community.MCPS.NotificationHandlers;

public class McpsUmbracoStartingNotification(
    ILogger<UmbracoApplicationStartedNotification> _logger,
    IShortStringHelper shortStringHelper,
    IDataTypeService dataTypeService,
    IMcpsDataTypeService mcpsDataTypeService,
    IUserService userService,
    IUmbracoDatabaseFactory umbracodatabaseFactory) : INotificationHandler<UmbracoApplicationStartedNotification>
{
    public async void Handle(UmbracoApplicationStartedNotification notification)
    {
        _logger.LogInformation("Mcps Umbraco Started Notification");

        if (!string.IsNullOrWhiteSpace(umbracodatabaseFactory.ConnectionString) && userService.GetUserById(-1) is IMembershipUser user)
        {
            try
            {
                var dropdownConfig = new Dictionary<string, object>
                {
                    { "items", new List<string>
                        {
                            {"Tomat" },
                            {"Ost" },
                            {"Kage" }
                        }
                    },
                    { "multiple", false }
                };

                var dt = await mcpsDataTypeService.CreateMcpsDataType("Food", "Dropdown", dropdownConfig, user.Key);

                var stringLabelDataType = await dataTypeService.GetAsync("Label (string)");
                if (stringLabelDataType is not null)
                {
                    var dataPropertyType = new PropertyType(shortStringHelper, stringLabelDataType) { Alias = McpsConstants.ReferenceIdAlias, Name = McpsConstants.ReferenceIdName };
                    var contentComposition = await mcpsDataTypeService.CreateMcpsContentType(user.Key, dataPropertyType, McpsConstants.PascalPrefix + " " + McpsConstants.ReferenceIdName, McpsConstants.CamelPrefix + McpsConstants.ReferenceIdAlias, McpsConstants.CompositionContainerName, null);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in McpsUmbracoStartingNotification.Handle");
            }
        }
    }
}