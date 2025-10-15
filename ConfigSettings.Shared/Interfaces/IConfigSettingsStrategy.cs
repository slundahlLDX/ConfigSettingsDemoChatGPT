using ConfigSettings.Shared.Models;

namespace ConfigSettings.Shared.Interfaces;

public interface IConfigSettingsStrategy
{
    Task<IEnumerable<ConfigSetting>> GetAllAsync();
    Task<ConfigSetting?> GetByIdAsync(int id);
    Task<int> AddAsync(ConfigSetting setting);
    Task<bool> UpdateAsync(ConfigSetting setting);
    Task<bool> DeleteAsync(int id);
}
