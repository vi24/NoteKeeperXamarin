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

        public Task Save<T>(T obj, string path)
        {
            _serializer = new DataContractSerializer(typeof(T));
            try
            {
                using (var stream = new StreamWriter(path))
                {
                    using (var writer = new XmlTextWriter(stream) { Formatting = Formatting.Indented })
                    {
                        _serializer.WriteObject(writer, obj);
                    }
                }
            }
            catch(DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            return Task.CompletedTask;
        }

        public Task<T> Open<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }
            _serializer = new DataContractSerializer(typeof(T));
            using (Stream stream = File.OpenRead(path))
            {
                T obj = (T)_serializer.ReadObject(stream);
                return Task.FromResult(obj);
            }
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
