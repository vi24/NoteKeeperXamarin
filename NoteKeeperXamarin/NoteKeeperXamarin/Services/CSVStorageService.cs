using CsvHelper;
using System;
using System.IO;



namespace NoteKeeperXamarin.Services
{
    public class CSVStorageService : IStorageService
    {
        public string FileExtensionName { get; }

        public CSVStorageService()
        {
            FileExtensionName = ".csv";
        }

        public void DeleteFile<T>(string path)
        {
            File.Delete(path);
        }

        public T OpenFile<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }

            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Read();
                    csv.Configuration.HasHeaderRecord = false;
                    T obj = csv.GetRecord<T>();
                    return obj;
                }
            }
        }

        public void SaveToFile<T>(T obj, string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                using (var csvWriter = new CsvWriter(file))
                {
                    csvWriter.WriteRecord(obj);
                }
            }
        }
    }
}
