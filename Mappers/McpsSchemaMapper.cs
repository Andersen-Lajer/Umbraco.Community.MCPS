using System.Text.Json;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Schemas;

namespace Umbraco.Community.MCPS.Mappers;

public class McpsSchemaMapper()
{
    public static PropagationSettingSchema MapToSchema(PropagationSetting propagationSetting)
    {
        var schema = new PropagationSettingSchema()
        {
            Name = propagationSetting.Name,
            PropagationPriority = JsonSerializer.Serialize(propagationSetting.PropagationPriority),
            DuplicationStrategy = (int)propagationSetting.DuplicationStrategy,
            PropertyAlias = propagationSetting.PropertyAlias,
            ContentTypes = JsonSerializer.Serialize(propagationSetting.ContentTypes)
        };

        if (propagationSetting.FallbackSetting is not null && propagationSetting.FallbackSetting.Id is int stFallbackId)
        {
            schema.FallbackId = stFallbackId;
        }
        return schema;
    }
}