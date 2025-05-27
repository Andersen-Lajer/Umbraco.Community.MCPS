using Umbraco.Community.MCPS.Models;


namespace Umbraco.Community.MCPS.Helpers;

// NOTE: This should be made redundant with time - Optimally all settings are handled generically down the line.
static class McpsSettingsHelper
{
    // NOTE: THIS IS A HARDCODED EXAMPLE OF HOW PROPAGATIONSETTINGS COULD BE SET UP.
    public static PropagationSetting McpsDropdownSetting = new()
    {
        Name = "Mcps Dropdown Propagation",
        ContentTypes = ["article", "homepage"],
        PropertyAlias = "mcpsDropdown"
    };


}