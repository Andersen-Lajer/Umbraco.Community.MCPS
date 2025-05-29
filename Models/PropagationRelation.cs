using Umbraco.Community.MCPS.Models.Schemas;

namespace Umbraco.Community.MCPS.Models;

public class PropagationRelation
{
    public PropagationRelation() { }
    public PropagationRelation(Guid pageId, int positionId, Guid referenceId, int propagationSettingId)
    {
        PageId = pageId;
        PositionId = positionId;
        ReferenceId = referenceId;
        PropagationSettingId = propagationSettingId;
    }

    public PropagationRelation(int id, Guid pageId, int positionId, Guid referenceId, int propagationSettingId)
    {
        Id = id;
        PageId = pageId;
        PositionId = positionId;
        ReferenceId = referenceId;
        PropagationSettingId = propagationSettingId;
    }

    public PropagationRelation(PropagationRelationsSchema propagationRelationsSchema)
    {
        Id = propagationRelationsSchema.Id;
        PositionId = propagationRelationsSchema.PositionId;
        PageId = propagationRelationsSchema.PageId;
        ReferenceId = propagationRelationsSchema.ReferenceId;
        PropagationSettingId = propagationRelationsSchema.PropagationSettingId;
    }
    public int? Id { get; set; }
    public Guid? PageId { get; set; }
    public int? PositionId { get; set; }
    public Guid? ReferenceId { get; set; }
    public int PropagationSettingId { get; set; }
    public string? Value { get; set; }
}