using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Community.MCPS.Models.Schemas;

[TableName("McpsPropagationRelations")]
[PrimaryKey("Id", AutoIncrement = true)]
[ExplicitColumns]
public class PropagationRelationsSchema
{
    [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
    [Column("Id")]
    public int Id { get; set; }
    [Column("PageId")]
    public Guid PageId { get; set; }
    [Column("PositionId")]
    public int PositionId { get; set; }
    [Column("PropagationSettingId")]
    [ForeignKey(typeof(PropagationSettingSchema), Name = "FK_PropagationRelation_PropagationSetting")]
    [Index(IndexTypes.NonClustered, Name = "IX_PropagationRelation_PropagationSettingId")]
    public int PropagationSettingId { get; set; }
    [Column("Value")]
    [NullSetting(NullSetting = NullSettings.Null)]
    public string? Value { get; set; }
    [Column("ReferenceId")]
    [NullSetting(NullSetting = NullSettings.Null)]
    public Guid? ReferenceId { get; set; }
}