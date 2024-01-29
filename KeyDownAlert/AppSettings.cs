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
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing;

namespace KeyDownAlert
{


    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public static class AppSettings
    {
        private static readonly string storagePath = "appSettings.json";
        private static readonly object locker = new object();

        public static void Save<T>(string key, T value)
        {
            lock (locker)
            {
                try
                {
                    var storage = File.Exists(storagePath)
                        ? JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(storagePath))
                        : new Dictionary<string, string>();

                    storage[key] = JsonConvert.SerializeObject(value);

                    File.WriteAllText(storagePath, JsonConvert.SerializeObject(storage));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while storing the object: {ex.Message}");
                }
            }
        }
        public static T Load<T>(string key)
        {
            lock (locker)
            {
                try
                {
                    if (File.Exists(storagePath))
                    {
                        var storage = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(storagePath));

                        if (storage.ContainsKey(key))
                        {
                            return JsonConvert.DeserializeObject<T>(storage[key]);
                        }
                        else
                        {
                            Console.WriteLine($"No object found with the key: {key}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The storage file does not exist.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving the object: {ex.Message}");
                }

                return default(T);
            }
        }
        public static void SaveSerialized<T>(string key, T value)
        {
            lock (locker)
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(key + ".dat", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, value);
                }
            }
        }
        public static T LoadSerialized<T>(string key)
        {
            lock (locker)
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


    public class EncryptionHelper
    {
        private readonly string EncryptionKey = Generate256BitKey();

        public EncryptionHelper() { EncryptionKey = Generate256BitKey(); }
        public EncryptionHelper(string encryptionKey) { EncryptionKey = encryptionKey; }

        private static string Generate256BitKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256; // Explicitly set key size
                aes.BlockSize = 128; // Explicitly set block size
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }

        public string GetEncryptionKey()
        {
            return EncryptionKey;
        }

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Explicitly set key size
                aesAlg.BlockSize = 128; // Explicitly set block size
                aesAlg.Key = Convert.FromBase64String(EncryptionKey);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Explicitly set key size
                aesAlg.BlockSize = 128; // Explicitly set block size
                aesAlg.Key = Convert.FromBase64String(EncryptionKey);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public void Show(string message)
        {
            string originalText = message;
            Console.WriteLine($"Original Text: {originalText}");

            string encryptedText = Encrypt(originalText);
            Console.WriteLine($"Encrypted Text: {encryptedText}");

            string decryptedText = Decrypt(encryptedText);
            Console.WriteLine($"Decrypted Text: {decryptedText}");
        }
    }
}
