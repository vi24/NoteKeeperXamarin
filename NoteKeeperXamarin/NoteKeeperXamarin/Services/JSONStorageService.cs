using Newtonsoft.Json;
using System;
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

        public Task<T> Open<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }
            using (var reader = new StreamReader(path))
            {
                using (var jreader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    T obj = serializer.Deserialize<T>(jreader);
                    return Task.FromResult(obj);
                }
            }
        }

        public Task Save<T>(T obj, string path)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, obj);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                throw;
            }
            return Task.CompletedTask;
        }

        public Task Delete<T>(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {

            }
            return Task.CompletedTask;
        }
    }
}
