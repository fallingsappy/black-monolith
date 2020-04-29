using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DDrop.Utility.SeriesLocalStorageOperations
{
    public static class JsonSerializeProvider
    {
        public static async Task SerializeToFileAsync<T>(T serializeObject, string path)
        {
            using (var fileStream = File.Create(path))
            {
                await JsonSerializer.SerializeAsync(fileStream, serializeObject);
            }
        }

        public static async Task<T> DeserializeFromFileAsync<T>(string path)
        {
            using (var fileStream = File.OpenRead(path))
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<T>(fileStream);
                }
                catch (JsonException)
                {
                    throw new InvalidOperationException();
                }
                
            }
        }

        public static string SerializeToString<T>(T classObject)
        {
            return JsonSerializer.Serialize(classObject);
        }

        public static T DeserializeFromString<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}