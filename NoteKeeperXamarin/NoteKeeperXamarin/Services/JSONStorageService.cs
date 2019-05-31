﻿using Newtonsoft.Json;
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
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                T obj = (T)serializer.Deserialize(file, typeof(T));
                return obj;
            }
        }

        public void SaveToFile<T>(T obj, string path)
        {
            using( StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, obj);
            }
        }

        public void DeleteFile<T>(string path)
        { 
            File.Delete(path);
        }
    }
}
