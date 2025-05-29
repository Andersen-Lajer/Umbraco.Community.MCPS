using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.MCPS.Services;
using Umbraco.Community.MCPS.NotificationHandlers;
using Umbraco.Community.MCPS.Repositories;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.DependencyInjection;

internal sealed class McpsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services
            .AddSingleton<ICompositionConfigurationService, CompositionConfigurationService>()
            .AddSingleton<IMcpsDataTypeService, McpsDataTypeService>()
            .AddSingleton<IMcpsDatabaseRepository, McpsDatabaseRepository>()
            .AddScoped<IMcpsPropagationService, McpsSearchService>()
            .AddScoped<IMcpsRelationService, McpsRelationService>()
            .AddScoped<IMcpsPropagationSettingService, McpsPropagationSettingService>()
            .AddScoped<IContentPropagationService, ContentPropagationService>()
            .AddScoped<IContentRetrievalService, ContentRetrievalService>()
            .AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder
            .AddNotificationHandler<UmbracoApplicationStartedNotification, McpsUmbracoStartingNotification>()
            .AddNotificationHandler<ContentSavingNotification, McpsContentSavingNotification>();
    }   
}
