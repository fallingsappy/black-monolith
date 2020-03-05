using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DDrop.Utility.SeriesLocalStorageOperations
{
    public static class LocalSeriesProvider
    {
        public static async Task SerializeAsync<T>(T serializeObject, string path)
        {
            using (FileStream fileStream = File.Create(path))
            {
                await JsonSerializer.SerializeAsync(fileStream, serializeObject);
            }            
        }

        public static async Task<T> DeserializeAsync<T>(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                return await JsonSerializer.DeserializeAsync<T>(fileStream);
            }
        }
    }
}