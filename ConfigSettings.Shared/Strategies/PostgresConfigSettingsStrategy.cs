using ConfigSettings.Shared.Interfaces;
using ConfigSettings.Shared.Models;
using ConfigSettings.Shared.Crypto;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace ConfigSettings.Shared.Strategies;

public class PostgresConfigSettingsStrategy : IConfigSettingsStrategy
{
    private readonly string _connectionString;
    private readonly CryptoHelper _crypto;
    private readonly ILogger<PostgresConfigSettingsStrategy>? _logger;

    public PostgresConfigSettingsStrategy(string connectionString, CryptoHelper cryptoHelper, ILogger<PostgresConfigSettingsStrategy>? logger = null)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _crypto = cryptoHelper ?? throw new ArgumentNullException(nameof(cryptoHelper));
        _logger = logger;
    }

    public async Task<IEnumerable<ConfigSetting>> GetAllAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<ConfigSetting>("SELECT * FROM settings.config_settings ORDER BY config_settings_id");

        foreach (var item in results)
        {
            if (item.IsEncrypted && !string.IsNullOrEmpty(item.Salt) && !string.IsNullOrEmpty(item.IV))
            {
                try
                {
                    var iv = CryptoHelper.IVFromBase64(item.IV!);
                    item.FieldValue = _crypto.Decrypt(item.FieldValue, item.Salt!, iv);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to decrypt config_settings_id {Id}", item.ConfigSettingsId);
                    throw;
                }
            }
        }

        return results;
    }

    public async Task<ConfigSetting?> GetByIdAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var item = await conn.QueryFirstOrDefaultAsync<ConfigSetting>("SELECT * FROM settings.config_settings WHERE config_settings_id = @id", new { id });
        if (item != null && item.IsEncrypted && !string.IsNullOrEmpty(item.Salt) && !string.IsNullOrEmpty(item.IV))
        {
            item.FieldValue = _crypto.Decrypt(item.FieldValue, item.Salt!, CryptoHelper.IVFromBase64(item.IV!));
        }
        return item;
    }

    public async Task<int> AddAsync(ConfigSetting setting)
    {
        if (setting.IsEncrypted)
        {
            setting.Salt ??= CryptoHelper.GenerateSalt();
            var iv = CryptoHelper.GenerateIV();
            setting.IV = CryptoHelper.IVToBase64(iv);
            setting.FieldValue = _crypto.Encrypt(setting.FieldValue, setting.Salt, iv);
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = @"
            INSERT INTO settings.config_settings
            (application_name, instance, hostname, username, classification, field_name, field_value,
             is_encrypted, salt, iv, comment, updated_by)
            VALUES (@ApplicationName, @Instance, @HostName, @UserName, @Classification, 
                    @FieldName, @FieldValue, @IsEncrypted, @Salt, @IV, @Comment, @UpdatedBy)
            RETURNING config_settings_id;";

        return await conn.ExecuteScalarAsync<int>(sql, setting);
    }

    public async Task<bool> UpdateAsync(ConfigSetting setting)
    {
        if (setting.IsEncrypted)
        {
            setting.Salt ??= CryptoHelper.GenerateSalt();
            var iv = CryptoHelper.GenerateIV();
            setting.IV = CryptoHelper.IVToBase64(iv);
            setting.FieldValue = _crypto.Encrypt(setting.FieldValue, setting.Salt, iv);
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = @"
            UPDATE settings.config_settings
            SET field_value = @FieldValue,
                is_encrypted = @IsEncrypted,
                salt = @Salt,
                iv = @IV,
                comment = @Comment,
                updated_by = @UpdatedBy,
                updated_date = now()
            WHERE config_settings_id = @ConfigSettingsId";
        return await conn.ExecuteAsync(sql, setting) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.ExecuteAsync("DELETE FROM settings.config_settings WHERE config_settings_id = @id", new { id }) > 0;
    }
}
