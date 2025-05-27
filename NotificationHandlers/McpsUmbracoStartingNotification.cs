using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Microsoft.Extensions.Logging;
using Umbraco.Community.MCPS.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Helpers;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Core.Models.Membership;

namespace Umbraco.Community.MCPS.NotificationHandlers;

public class McpsUmbracoStartingNotification(
    ILogger<UmbracoApplicationStartedNotification> _logger,
    IShortStringHelper _shortStringHelper,
    ICompositionConfigurationService compositionSvc,
    IDataTypeService dataTypeService,
    IMcpsDataTypeService _mcpsDataTypeService,
    IUserService userService,
    IUmbracoDatabaseFactory umbracodatabaseFactory) : INotificationHandler<UmbracoApplicationStartedNotification>
{
    public async void Handle(UmbracoApplicationStartedNotification notification)
    {
        _logger.LogInformation("Mcps Umbraco Started Notification");

        // NOTE: Consider what user GUID should be used here
        Guid userId = new();

        if (!string.IsNullOrWhiteSpace(umbracodatabaseFactory.ConnectionString) && userService.GetByUsername("admin@admin.dk") is IMembershipUser dummyAdminUser)
        {
            userId = dummyAdminUser.Key;
        }

        try
        {
            var dropdownConfig = new Dictionary<string, object>
            {
                { "items", new List<string>
                    {
                        {"Option 1" },
                        {"Option 2" },
                        {"Option 3" }
                    }
                },
            { "multiple", false }
            };

            var dt = await _mcpsDataTypeService.CreateMcpsDataType("Dropdown", dropdownConfig, userId);

            ContentType newCT = new(_shortStringHelper, -1) { Alias = ConstStrings.CamelPrefix + "DropdownComposition", Icon = "icon-clothes-hanger color-deep-purple" };
            newCT.Name = ConstStrings.PascalPrefix + " Dropdown Composition";
            newCT.IsElement = true;

            PropertyGroup pg = new(false) { Alias = "content", Name = "Content", Type = PropertyGroupType.Tab };

            if (pg.PropertyTypes is not null)
            {
                if (dt is not null) { pg.PropertyTypes.Add(new PropertyType(_shortStringHelper, dt) { Alias = ConstStrings.CamelPrefix + "Dropdown", Name = ConstStrings.PascalPrefix + "Dropdown" }); }

                newCT.PropertyGroups.Add(pg);
                compositionSvc.CreateComposition(newCT, userId);
            }

            var stringLabelDataType = await dataTypeService.GetAsync("Label (string)");
            if (stringLabelDataType is not null)
            {

                var dataPropertyType = new PropertyType(_shortStringHelper, stringLabelDataType) { Alias = ConstStrings.ReferenceIdAlias, Name = ConstStrings.ReferenceIdName };
                var contentComposition = await _mcpsDataTypeService.CreateMcpsContentType(userId, dataPropertyType, ConstStrings.PascalPrefix + " " + ConstStrings.ReferenceIdName, ConstStrings.CamelPrefix + ConstStrings.ReferenceIdAlias, ConstStrings.CompositionContainerName, null);
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}