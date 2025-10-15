using System.Net.Http.Json;
using ConfigSettings.Shared.Interfaces;
using ConfigSettings.Shared.Models;
using Microsoft.Extensions.Logging;

namespace ConfigSettings.Shared.Strategies;

public class ApiConfigSettingsStrategy : IConfigSettingsStrategy
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiConfigSettingsStrategy> _logger;

    public ApiConfigSettingsStrategy(HttpClient http, ILogger<ApiConfigSettingsStrategy> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IEnumerable<ConfigSetting>> GetAllAsync()
    {
        _logger.LogInformation("Calling API: GetAll");
        var result = await _http.GetFromJsonAsync<IEnumerable<ConfigSetting>>("api/configsettings");
        return result ?? Enumerable.Empty<ConfigSetting>();
    }

    public async Task<ConfigSetting?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Calling API: GetById {Id}", id);
        return await _http.GetFromJsonAsync<ConfigSetting>($"api/configsettings/{id}");
    }

    public async Task<int> AddAsync(ConfigSetting setting)
    {
        _logger.LogInformation("Calling API: Add");
        var response = await _http.PostAsJsonAsync("api/configsettings", setting);
        response.EnsureSuccessStatusCode();
        int? val = await response.Content.ReadFromJsonAsync<int>();
        return val ?? 0;
    }

    public async Task<bool> UpdateAsync(ConfigSetting setting)
    {
        _logger.LogInformation("Calling API: Update {Id}", setting.ConfigSettingsId);
        var response = await _http.PutAsJsonAsync($"api/configsettings/{setting.ConfigSettingsId}", setting);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Calling API: Delete {Id}", id);
        var response = await _http.DeleteAsync($"api/configsettings/{id}");
        return response.IsSuccessStatusCode;
    }
}
