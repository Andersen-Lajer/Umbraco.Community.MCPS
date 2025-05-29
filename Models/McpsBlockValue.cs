using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Umbraco.Community.MCPS.Models;

public class McpsBlockValue
{
    [JsonProperty("Layout")]
    public IDictionary<string, JToken> Layout { get; set; } = null!;
    [JsonProperty("contentData")]
    public List<McpsBlockItemData> ContentData { get; set; } = [];
    [JsonProperty("settingsData")]
    public List<McpsBlockItemData> SettingsData { get; set; } = [];
    [JsonProperty("expose")]
    public List<Dictionary<string, object?>> Expose { get; set; } = [];

    public bool Update(Dictionary<Guid, Dictionary<string, string>> insertValues, Dictionary<Guid, string> referencePropertyTypes)
    {
        foreach (var blockItemData in ContentData)
        {
            if (blockItemData.Values is null
                    || blockItemData.Values.FirstOrDefault(x => x.Alias == McpsConstants.ReferenceIdAlias) is not null
                    || blockItemData.ContentTypeKey is not Guid contentTypeKey) { continue; }

            string propagationSettings = "";
            if (blockItemData.Values is not List<McpsBlockPropertyValue> blockValues) { continue; }
            if (blockValues.FirstOrDefault(x => x.Alias == referencePropertyTypes[contentTypeKey]) is McpsBlockPropertyValue prop && prop.Value is not null)
            {
                propagationSettings = prop.Value;
            }
            blockItemData.UpdateValues(insertValues[contentTypeKey]);
        }
        return true;
    }
}