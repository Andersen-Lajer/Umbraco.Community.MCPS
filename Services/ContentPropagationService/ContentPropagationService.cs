using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Enums;
using Umbraco.Community.MCPS.Models.Schemas;
using Umbraco.Community.MCPS.Repositories;

namespace Umbraco.Community.MCPS.Services;

class ContentPropagationService(
    IMcpsDatabaseRepository repository,
    IMcpsRelationService relationService,
    IMcpsPropagationService propagationService,
    ILogger<ContentPropagationService> logger) : IContentPropagationService
{
    public bool PropagateSavedContent(IContent savedEntity, List<PropagationSetting> propagationSettings)
    {
        if (!savedEntity.Properties.Where(x => x.Values.Count != 0).Any())
        {
            logger.LogInformation("No properties to propagate");
            return false;
        }

        var positionCount = relationService.UpdateContentRelations(savedEntity, propagationSettings);
        foreach (var setting in propagationSettings)
        {
            PropagateSetting(setting);
        }

        return true;
    }

    public bool CreatePropagationSetting(PropagationSetting setting)
    {
        try
        {
            logger.LogInformation("Calling CreatePropagationSetting from ContentPropagationService");
            repository.CreatePropagationSetting(setting);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CreatePropagationSetting");
            return false;
        }
    }

    public bool PropagateSetting(PropagationSetting setting)
    {
        switch (setting.DuplicationStrategy)
        {
            case DuplicationStrategy.UniquePage:
                PropagateUniquePage(setting);
                break;
            case DuplicationStrategy.UniqueProperty:
                PropagateUniqueProperty(setting);
                break;
            case DuplicationStrategy.UniqueSite:
                PropagateUniqueSite(setting);
                break;
            default:
                break;
        }

        return true;
    }

    public async Task<bool> PropagateUniquePage(PropagationSetting setting)
    {
        if (setting.Id is not int settingId) { return false; }

        var relations = repository.GetPropagationRelations(settingId);

        Dictionary<Guid, Dictionary<string, List<PropagationRelationsSchema>>> pagedRelations = [];

        Dictionary<string, int> valueCounts = [];


        foreach (var relation in relations)
        {
            relation.Value ??= "NULL";

            if (valueCounts.TryGetValue(relation.Value, out int valueCount))
            {
                valueCount++;
                valueCounts[relation.Value] = valueCount;
            }
            else
            {
                valueCounts.Add(relation.Value, 1);
            }

            if (pagedRelations.ContainsKey(relation.PageId))
            {

                if (pagedRelations[relation.PageId].TryGetValue(relation.Value, out List<PropagationRelationsSchema>? value))
                {
                    value.Add(relation);
                }
                else
                {
                    pagedRelations[relation.PageId].Add(relation.Value, [relation]);
                }
            }
            else
            {
                pagedRelations.Add(relation.PageId, new()
                {
                    {relation.Value, [relation] }
                });
            }
        }

        List<PropagationRelationsSchema> updatedRelations = [];

        foreach (var value in valueCounts)
        {
            var references = propagationService.GetContentReferences(0, value.Value, value.Key, setting, setting.PropertyAlias);
            if (references.Count == 0)
            {
                logger.LogWarning("No references found for value: {value}", value.Key);
                continue;
            }

            var pagedValues = pagedRelations.Values.Where(x => x.ContainsKey(value.Key)).Select(y => y[value.Key]);
            foreach (var pagedValue in pagedValues)
            {
                List<Guid> tmpReferences = [.. references];
                foreach (var relation in pagedValue)
                {
                    if (tmpReferences.Count == 0) { break; }
                    relation.ReferenceId = tmpReferences.FirstOrDefault();
                    tmpReferences.RemoveAt(0);
                }
                updatedRelations.AddRange(pagedValue);
            }
        }
        repository.UpdatePropagationRelations(updatedRelations);

        return true;
    }
    public bool PropagateUniqueProperty(PropagationSetting setting)
    {
        throw new NotImplementedException();
    }
    public bool PropagateUniqueSite(PropagationSetting setting)
    {
        throw new NotImplementedException();
    }
}