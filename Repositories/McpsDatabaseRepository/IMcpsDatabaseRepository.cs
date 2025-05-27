using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Schemas;

namespace Umbraco.Community.MCPS.Repositories;

public interface IMcpsDatabaseRepository
{
    // PropagationRelation
    public int? CreatePropagationRelation(PropagationRelationsSchema relation);
    //public bool UpdatePropagationRelations(List<PropagationRelation> propagationRelations);
    public bool UpdatePropagationRelations(List<PropagationRelationsSchema> propagationRelations);
    public int RemoveUnusedRelations(Guid pageId, int relationCount);
    public Guid GetTargetGuid(string id);
    //public List<PropagationRelation> GetEmptyReferences();
    public List<PropagationRelationsSchema> GetPropagationRelations(int settingId);

    // PropagationSetting
    public PropagationSettingSchema CreatePropagationSetting(PropagationSetting propagationSetting);
    public PropagationSettingSchema CreatePropagationSetting(PropagationSettingSchema propagationSetting);
    public PropagationSettingSchema GetPropagationSetting(int propagationSettingId);
    public PropagationSettingSchema GetPropagationSetting(string propagationSettingName);
    public List<PropagationSettingSchema> GetPropagationSettingsByDocumentType(string documentType);
    public List<PropagationSettingSchema> GetAllPropagationSettings();

}
