using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace NoteKeeperXamarin.Services
{
    public class JSONStorageService : IStorageService
    {
        public string FileExtensionName { get; }

        public JSONStorageService()
        {
            FileExtensionName = ".json";
        }

        public async Task<T> Open<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }
            using (var reader = new StreamReader(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                T obj = (T)await (Task.Run(() => serializer.Deserialize(reader, typeof(T))));
                return obj;
            }
        }

        public async Task Save<T>(T obj, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                await Task.Run(() => serializer.Serialize(writer, obj));
            }
        }

        public async Task Delete<T>(string path)
        {
            await Task.Run(() => File.Delete(path));
        }
    }
}
