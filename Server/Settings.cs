using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
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
        public string MulticastIPAddress { get; set; }
        public int MulticastPort { get; set; }
        public int Port { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
