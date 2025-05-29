using System.Text.Json;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Schemas;
using Umbraco.Community.MCPS.Models.Enums;
using Umbraco.Community.MCPS.Repositories;

namespace Umbraco.Community.MCPS.Mappers;

public class McpsServiceModelMapper(IMcpsDatabaseRepository _repository)
{
    public PropagationSetting MapToServiceModel(PropagationSettingSchema schema)
    {
        var viewModel = new PropagationSetting()
        {
            Id = schema.Id,
            Name = schema.Name,
            PropagationPriority = JsonSerializer.Deserialize<List<PropagationRank>>(schema.PropagationPriority) ?? [],
            DuplicationStrategy = (DuplicationStrategy)schema.DuplicationStrategy,
            PropertyAlias = schema.PropertyAlias,
            ContentTypes = JsonSerializer.Deserialize<List<string>>(schema.ContentTypes) ?? []
        };
        if (schema.FallbackId is int stFallbackId)
        {
            viewModel.FallbackSetting = MapToServiceModel(_repository.GetPropagationSetting(stFallbackId));
        }
        return viewModel;
    }
}