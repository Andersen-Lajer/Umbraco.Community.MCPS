using Newtonsoft.Json;

namespace Umbraco.Community.MCPS.Models;

public class McpsBlockPropertyValue
{
    [JsonProperty("editorAlias")]
    public string? EditorAlias { get; set; }
    [JsonProperty("culture")]
    public string? Culture { get; set; }
    [JsonProperty("segment")]
    public string? Segment { get; set; }
    [JsonProperty("alias")]
    public string? Alias { get; set; }
    [JsonProperty("value")]
    public string? Value { get; set; }
}
