using Newtonsoft.Json;

namespace Umbraco.Community.MCPS.Models;

public class McpsBlockItemData
{
    [JsonProperty("contentTypeKey")]
    public Guid? ContentTypeKey { get; set; }
    [JsonProperty("udi")]
    public string? Udi { get; set; }
    [JsonProperty("key")]
    public Guid? Key { get; set; }
    [JsonProperty("values")]
    public List<McpsBlockPropertyValue>? Values { get; set; }


    public bool UpdateValues(Dictionary<string, string> insertValues)
    {
        Values ??= [];

        foreach (var insertValue in insertValues)
        {
            if (Values.FirstOrDefault(x => x.Alias == insertValue.Key) is McpsBlockPropertyValue propertyValue)
            {
                propertyValue.Value = insertValue.Value;
            }
            else
            {
                Values.Add(new McpsBlockPropertyValue
                {
                    Alias = insertValue.Key,
                    Value = insertValue.Value
                });
            }
        }
        return true;
    }
}
