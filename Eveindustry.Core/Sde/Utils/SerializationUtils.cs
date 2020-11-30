using System;
using System.IO;
using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Core
{
    /// <summary>
    /// Helper methods for SDE serialization/deserialization/caching.
    /// </summary>
    public static class SerializationUtils
    {
        /// <summary>
        /// Read SDE data from eve YAML file. if cache file with given filename exists, read from binary cache instead.
        /// If cache file does not exist, creates it, so next time it will read from binary serialized cache,
        /// which is much faster.
        /// </summary>
        /// <param name="sdePath">full path to sde file. </param>
        /// <param name="cacheFileName">file name for binary serialization cache. </param>
        /// <typeparam name="T">type of requested data. </typeparam>
        /// <returns>requested SDE data read from yaml or binary cache. </returns>
        public static T ReadAndCacheBinary<T>(string sdePath, string cacheFileName)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var fullCachePath = Path.Join(currentDir, cacheFileName);
            if (File.Exists(fullCachePath))
            {
                return ReadFromBinary<T>(fullCachePath);
            }

            var result = ReadFromYaml<T>(sdePath);
            DumpBinary(fullCachePath, result);
            return result;
        }

        private static T ReadFromYaml<T>(string filePath)
        {
            var doc = File.OpenRead(filePath);
            var serializer = new Deserializer();
            var yamlData = serializer.Deserialize<T>(new StreamReader(doc));
            return yamlData;
        }

        private static void DumpBinary<T>(string filePath, T obj)
        {
            var bytes = MessagePackSerializer.Serialize(obj);
            File.WriteAllBytes(filePath, bytes);
        }

        private static T ReadFromBinary<T>(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            return MessagePackSerializer.Deserialize<T>(bytes);
        }
    }
}