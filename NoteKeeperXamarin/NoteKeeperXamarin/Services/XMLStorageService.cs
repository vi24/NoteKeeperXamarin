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

        public void SaveToFile(object obj, string path, Type type)
        {
            _serializer = new DataContractSerializer(type);
            using (var stream = new StreamWriter(path))
            {
                using (var writer = new XmlTextWriter(stream) { Formatting = Formatting.Indented })
                {
                    _serializer.WriteObject(writer, obj);
                }
            }
        }

        public object OpenFile(string path, Type type)
        {
            _serializer = new DataContractSerializer(type);
            using (Stream stream = File.OpenRead(path))
            {
                Object obj = _serializer.ReadObject(stream);
                return obj;
            }
        }
    }
}
