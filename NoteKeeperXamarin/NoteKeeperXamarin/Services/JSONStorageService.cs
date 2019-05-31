using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace NoteKeeperXamarin.Services
{
    public class JSONStorageService : IStorageService
    {
        private DataContractJsonSerializer _serializer;
        public string FileExtensionName { get; }

        public JSONStorageService()
        {
            FileExtensionName = ".json";
        }

        public T OpenFile<T>(string path)
        {
            _serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = File.Open(path, FileMode.Open))
            {
                using (var reader = JsonReaderWriterFactory.CreateJsonReader(stream, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null))
                {
                    T obj = (T) _serializer.ReadObject(reader);
                    return obj;
                }
            }
        }

        public void SaveToFile<T>(T obj, string path)
        {
            _serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8))
                {
                    _serializer.WriteObject(writer, obj);
                }
            }
        }
    }
}
