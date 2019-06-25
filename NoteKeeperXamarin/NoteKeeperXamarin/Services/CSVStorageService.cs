using CsvHelper;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NoteKeeperXamarin.Services
{
    public class CSVStorageService : IStorageService
    {
        public string FileExtensionName { get; }

        public CSVStorageService()
        {
            FileExtensionName = ".csv";
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

        public async Task<T> Open<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }

            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader))
                {
                    await csv.ReadAsync();
                    csv.Configuration.HasHeaderRecord = false;
                    T obj = csv.GetRecord<T>();
                    return obj;
                }
            }
        }

        public Task Save<T>(T obj, string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                using (var csvWriter = new CsvWriter(file))
                {
                     csvWriter.WriteRecord(obj);
                }
            }
            return Task.CompletedTask;
        }
    }
}
