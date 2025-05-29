using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Helpers;

static class McpsSettingsHelper
{
    public static PropagationSetting McpsDropdownSetting = new()
    {
        Name = "Mcps Dropdown Propagation",
        ContentTypes = ["article", "homepage"],
        PropertyAlias = "mcpsDropdown"
    };
}