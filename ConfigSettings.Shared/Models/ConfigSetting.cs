namespace ConfigSettings.Shared.Models;

public class ConfigSetting
{
    public int ConfigSettingsId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string FieldValue { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public string? Salt { get; set; }
    public string? IV { get; set; }
    public string? Comment { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
}
