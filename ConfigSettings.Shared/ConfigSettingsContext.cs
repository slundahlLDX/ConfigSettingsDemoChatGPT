using ConfigSettings.Shared.Interfaces;
using ConfigSettings.Shared.Models;

namespace ConfigSettings.Shared;

public class ConfigSettingsContext
{
    private IConfigSettingsStrategy _strategy;

    public ConfigSettingsContext(IConfigSettingsStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IConfigSettingsStrategy strategy) => _strategy = strategy;

    public Task<IEnumerable<ConfigSetting>> GetAllAsync() => _strategy.GetAllAsync();
    public Task<ConfigSetting?> GetByIdAsync(int id) => _strategy.GetByIdAsync(id);
    public Task<int> AddAsync(ConfigSetting setting) => _strategy.AddAsync(setting);
    public Task<bool> UpdateAsync(ConfigSetting setting) => _strategy.UpdateAsync(setting);
    public Task<bool> DeleteAsync(int id) => _strategy.DeleteAsync(id);
}
