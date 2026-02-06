using System.Collections.Generic;

namespace org.testar.settings.extended
{
    public class ExtendedSettingContainer
    {
        private readonly List<IExtendedSetting> settings = new List<IExtendedSetting>();

        public void add(IExtendedSetting setting)
        {
            settings.Add(setting);
        }
    }
}
