using System;
using System.IO;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using YamlDotNet.Serialization;

namespace Eveindustry.Sde.Utils
{
    /// <summary>
    /// Helper methods for SDE serialization/deserialization/caching.
    /// </summary>
    internal static class SerializationUtils
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
        public static async Task<T> ReadAndCacheBinaryAsync<T>(string sdePath, string cacheFileName)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var fullCachePath = Path.Join(currentDir, cacheFileName);
            if (File.Exists(fullCachePath))
            {
                return await ReadFromBinary<T>(fullCachePath);
            }

            var result = ReadFromYaml<T>(sdePath);
            await DumpBinary(fullCachePath, result);
            return result;
        }

        private static T ReadFromYaml<T>(string filePath)
        {
            var doc = File.OpenRead(filePath);
            var serializer = new Deserializer();
            var yamlData = serializer.Deserialize<T>(new StreamReader(doc));
            return yamlData;
        }

        public static async Task DumpBinary<T>(string filePath, T obj)
        {
            await using var file = File.Create(filePath);
            await MessagePackSerializer.SerializeAsync(file, obj, ContractlessStandardResolverAllowPrivate.Options);
            file.Flush();
        }

        public static async Task<T> ReadFromBinary<T>(string filePath)
        {
            await using var bytes = File.OpenRead(filePath);
            return await MessagePackSerializer.DeserializeAsync<T>(bytes, ContractlessStandardResolverAllowPrivate.Options);
        }
    }
}