using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    [Serializable]
    public class Settings
    {
        private static Settings settings;
        private Settings() { }

        public static Settings GetSettings()
        {
            if (settings == null)
                settings = new Settings();

            return settings;
        }
        public string ServerCore { get; set; }
        public string DataBase { get; set; }

        public string MulticastIPAddress { get; set; }
        public int MulticastPort { get; set; }
    }
}
