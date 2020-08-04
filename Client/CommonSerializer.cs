using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Client
{
    class CommonSerializer
    {
        public static void Serialize<T>(T obj, string path)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, obj);
            }
        }

        public static T Deserialize<T>(string path)
        {
            if (File.Exists(path))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));

                T obj = default;

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    obj = (T)formatter.Deserialize(fs);
                }

                return obj;
            }

            return default;
        }
    }
}
