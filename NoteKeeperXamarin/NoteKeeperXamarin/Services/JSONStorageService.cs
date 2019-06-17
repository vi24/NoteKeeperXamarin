using Newtonsoft.Json;
using System.IO;

namespace NoteKeeperXamarin.Services
{
    public class JSONStorageService : IStorageService
    {
        public string FileExtensionName { get; }

        public JSONStorageService()
        {
            FileExtensionName = ".json";
        }

        public T OpenFile<T>(string path)
        {
            using (var reader= new StreamReader(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                T obj = (T)serializer.Deserialize(reader, typeof(T));
                return obj;
            }
        }

        public void SaveToFile<T>(T obj, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }
        }

        public void DeleteFile<T>(string path)
        { 
            File.Delete(path);
        }
    }
}
