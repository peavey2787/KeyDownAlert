using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace KeyDownAlert
{
    internal static class AppsSettings
    {
        public static void Save<T>(string key, T value)
        {
            string serializedValue = JsonConvert.SerializeObject(value);
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Check if the key already exists in AppSettings
            KeyValueConfigurationElement key_config = config.AppSettings.Settings[key];

            // If the key is not found, add it to AppSettings
            if (key_config == null)
            {
                config.AppSettings.Settings.Add(key, serializedValue);
            }
            else
            {
                key_config.Value = serializedValue;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        public static T Load<T>(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Check if the key exists in AppSettings
            KeyValueConfigurationElement key_config = config.AppSettings.Settings[key];

            if (key_config == null)
            {
                return default(T);
            }
            else
            {
                string serializedValue = key_config.Value;
                var deserialized = JsonConvert.DeserializeObject<T>(serializedValue);
                return deserialized;
            }
        }

        public static void SaveSerialized<T>(string key, T value)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(key + ".dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, value);
            }
        }

        public static T LoadSerialized<T>(string key)
        {
            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(key + ".dat", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (stream.Length > 0)
                    {
                        return (T)formatter.Deserialize(stream);
                    }
                    else
                    {
                        return default(T); // Assuming T is a reference type, this will return null.
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // Handle the case where the file is not found (key doesn't exist)
                return default(T);
            }
        }
    }
}
