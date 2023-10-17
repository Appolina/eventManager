
namespace EventManager.Model
{
    public delegate void SettingValueChanged(string settingName, object settingValue);

    public interface ISettingsModel 
    {
        int? GroupId { get; set; }
        string SignSuffix { get; set; }

        event SettingValueChanged OnSettingValueChanged;
    }
}
