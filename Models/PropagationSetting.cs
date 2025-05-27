
using Umbraco.Community.MCPS.Models.Enums;

namespace Umbraco.Community.MCPS.Models;

public class PropagationSetting
{
    public int? Id { get; set; }
    // Contains the rules defining the setting itself
    public required string Name { get; set; }
    public List<PropagationRank> PropagationPriority { get; set; } = [PropagationRank.Relevant, PropagationRank.Newest, PropagationRank.MostPopular];
    public DuplicationStrategy DuplicationStrategy { get; set; } = DuplicationStrategy.UniquePage;
    public required List<string> ContentTypes { get; set; }
    public required string PropertyAlias { get; set; }
    public PropagationSetting? FallbackSetting { get; set; }
}