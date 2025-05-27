

using Umbraco.Community.MCPS.Models;

namespace Umbraco.Community.MCPS.Repositories.McpsContentPickerRepository;

interface IMcpsContentPickerRepository
{
    // NOTE: Change parameter name, it's awful 

    // NOTE: Consider making a solid wrapper for this method - To log time of update and content updated. 
    public Dictionary<Guid, List<Guid>> GetContent(Dictionary<PropagationSetting, int> settingsWithAmounts);
}
