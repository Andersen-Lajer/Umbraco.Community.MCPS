using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Community.MCPS.Models;

[TableName("McpsPropagationSettings")]
[PrimaryKey("Id", AutoIncrement = true)]
[ExplicitColumns]
public class PropagationSettingSchema
{
    [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
    [Column("Id")]
    public int Id { get; set; }
    
    [Column("Name")]
    public required string Name { get; set; }

    [Column("PropagationPriority")]
    public required string PropagationPriority { get; set; }

    [Column("DuplicationStrategy")]
    public required int DuplicationStrategy { get; set; }

    [Column("ContentTypes")]
    public required string ContentTypes { get; set; }

    [Column("PropertyAlias")]
    public required string PropertyAlias { get; set; }

    [Column("FallbackSetting")]
    [NullSetting(NullSetting = NullSettings.Null)]
    public int? FallbackId { get; set; }
}