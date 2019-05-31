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

        public Object OpenFile(string path, Type type)
        {
            _serializer = new DataContractJsonSerializer(type);
            using (var stream = File.Open(path, FileMode.Open))
            {
                using (var reader = JsonReaderWriterFactory.CreateJsonReader(stream, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null))
                {
                    Object obj = _serializer.ReadObject(reader);
                    return obj;
                }
            }
        }

        public void SaveToFile(Object obj, string path, Type type)
        {
            _serializer = new DataContractJsonSerializer(type);
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
