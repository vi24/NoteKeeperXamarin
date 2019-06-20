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

        public async Task DeleteFile<T>(string path)
        {
            await Task.Run(() => File.Delete(path));
        }

        public async Task<T> OpenFile<T>(string path)
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
                    return await Task.FromResult(obj);
                }
            }
        }

        public async Task SaveToFile<T>(T obj, string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                using (var csvWriter = new CsvWriter(file))
                {
                    await Task.Run(() => csvWriter.WriteRecord(obj));
                }
            }
        }
    }
}
