using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Repositories;

namespace Umbraco.Community.MCPS.Services;

public class McpsRelationService(IMcpsDatabaseRepository repository, ILogger<McpsRelationService> logger) : IMcpsRelationService
{
    public int UpdateContentRelations(IContent content, List<PropagationSetting> propagationSettings)
    {
        logger.LogInformation("Calling: McpsRelationService.UpdateContentRelations({Guid})", content.Key);

        int positionCount = 0;
        var properties = content.Properties.Where(x => propagationSettings.Where(y => y.PropertyAlias == x.Alias).Any()).ToList();

        foreach (var property in content.Properties)
        {
            switch ((property.PropertyType.PropertyEditorAlias)
)
            {
                case "Umbraco.BlockGrid":
                    positionCount += UpdateBlockGridRelations(property, propagationSettings, content.Key, positionCount);
                    break;
                default:
                    break;
            }
        }
        logger.LogInformation("Returning: McpsRelationService.UpdateContentRelations({Guid})", content.Key);
        repository.RemoveUnusedRelations(content.Key, positionCount);
        return positionCount;
    }

    private int UpdateBlockGridRelations(IProperty property, List<PropagationSetting> propagationSettings, Guid contentGuid, int positionCount)
    {
        foreach (var value in property.Values)
        {
            if (value.EditedValue is null || value.EditedValue.ToString() is not string valueString) { continue; }
            var blockValue = JsonConvert.DeserializeObject<McpsBlockValue>(valueString);

            if (blockValue is null) { continue; }

            foreach (var token in blockValue.Layout.Values.Children())
            {
                var tokenKey = token.Value<string>("contentKey");
                var blockItemData = blockValue.ContentData.Where(x => x.Key.ToString() == tokenKey).FirstOrDefault();
                if (blockItemData is null || blockItemData.Values is not List<McpsBlockPropertyValue> blockValues) { continue; }

                var propagationProperty = blockValues.Where(x => propagationSettings.Where(y => y.PropertyAlias == x.Alias).Any()).FirstOrDefault();
                if (propagationProperty is null) { continue; }

                var setting = propagationSettings.Where(x => x.PropertyAlias == propagationProperty.Alias).FirstOrDefault();

                if (setting is not null && setting.Id is int settingId)
                {
                    var referenceId = repository.CreatePropagationRelation(new() { PageId = contentGuid, PositionId = positionCount, PropagationSettingId = settingId, Value = propagationProperty.Value });

                    blockItemData.UpdateValues(new() { { McpsConstants.ReferenceIdAlias, $"{referenceId}" } });
                }
                positionCount++;
            }
            value.EditedValue = JsonConvert.SerializeObject(blockValue);
        }
        return positionCount;
    }
}