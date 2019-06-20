using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace NoteKeeperXamarin.Services
{
    public class XMLStorageService : IStorageService
    {
        private DataContractSerializer _serializer;
        public string FileExtensionName { get; }

        public XMLStorageService()
        {
            FileExtensionName = ".xml";
        }

        public async Task SaveToFile<T>(T obj, string path)
        {
            _serializer = new DataContractSerializer(typeof(T));
            using (var stream = new StreamWriter(path))
            {
                using (var writer = new XmlTextWriter(stream) { Formatting = Formatting.Indented })
                {
                    await Task.Run(() =>_serializer.WriteObject(writer, obj));
                }
            }
        }

        public async Task<T> OpenFile<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }
            _serializer = new DataContractSerializer(typeof(T));
            using (Stream stream = File.OpenRead(path))
            {
                T obj = (T) await Task.Run(() => _serializer.ReadObject(stream));
                return obj;
            }
        }

        public async Task DeleteFile<T>(string path)
        {
            await Task.Run(() => File.Delete(path));
        }
    }
}
