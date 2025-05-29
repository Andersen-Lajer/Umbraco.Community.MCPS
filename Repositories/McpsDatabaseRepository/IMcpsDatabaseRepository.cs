using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Schemas;

namespace Umbraco.Community.MCPS.Repositories;

public interface IMcpsDatabaseRepository
{
    int? CreatePropagationRelation(PropagationRelationsSchema relation);
    bool UpdatePropagationRelations(List<PropagationRelationsSchema> propagationRelations);
    int RemoveUnusedRelations(Guid pageId, int relationCount);
    Guid GetTargetGuid(string id);
    List<PropagationRelationsSchema> GetPropagationRelations(int settingId);
    PropagationSettingSchema CreatePropagationSetting(PropagationSetting propagationSetting);
    PropagationSettingSchema CreatePropagationSetting(PropagationSettingSchema propagationSetting);
    PropagationSettingSchema GetPropagationSetting(int propagationSettingId);
    PropagationSettingSchema GetPropagationSetting(string propagationSettingName);
    List<PropagationSettingSchema> GetPropagationSettingsByDocumentType(string documentType);
    List<PropagationSettingSchema> GetAllPropagationSettings();
}