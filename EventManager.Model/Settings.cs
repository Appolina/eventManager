namespace EventManager.Model
{
    public class Settings : ISettingsModel
    {
        private int? groupId;
        private string signSuffix;

        public Settings(int? groupId, string signSuffix)
        {
            this.groupId = groupId;
            this.signSuffix = signSuffix;
        }

        public int? GroupId
        {
            get
            {
                return groupId;
            }

            set
            {
                groupId = value;
                if (OnSettingValueChanged != null)
                    OnSettingValueChanged("GroupId", value);
            }
        }

        public string SignSuffix
        {
            get
            {
                return signSuffix;
            }

            set
            {
                signSuffix = value;
                if (OnSettingValueChanged != null)
                    OnSettingValueChanged("SignSuffix", value);
            }
        }



        public event SettingValueChanged OnSettingValueChanged;
    }
}
