using System;
using System.IO;
using System.Runtime.Serialization;
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

        public void SaveToFile<T>(T obj, string path)
        {
            _serializer = new DataContractSerializer(typeof(T));
            using (var stream = new StreamWriter(path))
            {
                using (var writer = new XmlTextWriter(stream) { Formatting = Formatting.Indented })
                {
                    _serializer.WriteObject(writer, obj);
                }
            }
        }

        public T OpenFile<T>(string path)
        {
            _serializer = new DataContractSerializer(typeof(T));
            using (Stream stream = File.OpenRead(path))
            {
                T obj = (T) _serializer.ReadObject(stream);
                return obj;
            }
        }

        public void DeleteFile<T>(string path)
        {
            File.Delete(path);
        }
    }
}
