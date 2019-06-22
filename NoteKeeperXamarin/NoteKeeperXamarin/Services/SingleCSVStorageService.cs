using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NoteKeeperXamarin.Services
{
    public class SingleCSVStorageService : IStorageService
    {
        private string _filePath;
        public string FileExtensionName { get; }



        public SingleCSVStorageService()
        {
            FileExtensionName = ".csv";
            //_filePath = Path.Combine()
        }

        public Task Delete<T>(string name)
        {
            throw new NotImplementedException();
        }

        public Task<T> Open<T>(string name)
        {
            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.HasHeaderRecord = false;
                    noteList = csv.GetRecords<Note>().ToList();
                }
            }
        }

        public async Task Save<T>(T obj, string filename)
        {
            using (StreamWriter file = File.AppendText(filename))
            {
                using (var csvWriter = new CsvWriter(file))
                { 
                    await Task.Run(() => csvWriter.WriteRecord(obj));
                    csvWriter.NextRecord();
                }
            }
        }
    }
}
