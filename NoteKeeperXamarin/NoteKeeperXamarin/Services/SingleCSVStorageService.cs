using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class SingleCSVStorageService : IStorageService
    {
        private readonly char _delimiter = ',';
        private readonly string _filePath;
        private readonly string _idname = "UniqueName";

        public SingleCSVStorageService(string path = null)
        {
            FileExtensionName = ".csv";
            if (path == null)
            {
                path = Path.Combine(FileSystem.AppDataDirectory, "SerializedNotes");
            }
            string filename = "notes";
            _filePath = Path.Combine(path, filename + FileExtensionName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string FileExtensionName { get; }

        public Task Delete<T>(string name)
        {
            if (!File.Exists(_filePath)) return Task.CompletedTask;
            try
            {
                int index;
                List<string> lines = File.ReadAllLines(_filePath).ToList();
                List<string> line = lines[0].Split(_delimiter).ToList();
                if (!line.Contains(_idname)) return Task.CompletedTask;
                index = line.IndexOf(_idname);
                foreach (string record in lines)
                {
                    line = record.Split(_delimiter).ToList();
                    if (line[0].ToString() == name)
                    {
                        lines.Remove(record);
                        break;
                    }
                }
                File.WriteAllLines(_filePath, lines.ToArray());
            }
            catch (Exception)
            {
                //Silently ignoring
            }
            return Task.CompletedTask;

        }

        public async Task<T> Open<T>(string name)
        {
            string[] headers = await ReadHeaders<T>();
            List<string> fields = new List<string>();
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.Delimiter = _delimiter.ToString();
                        csv.Read();
                        csv.ReadHeader();
                        csv.Configuration.HasHeaderRecord = true;
                        while (await csv.ReadAsync())
                        {
                            if (csv.GetField<string>(_idname) == name)
                            {
                                foreach (string header in headers)
                                {
                                    if (header == _idname) continue;
                                    fields.Add(csv.GetField<string>(header));
                                }
                                return (T)Activator.CreateInstance(typeof(T), fields.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Silently ignoring
            }
            return (T)Activator.CreateInstance(typeof(T), fields.ToArray());
        }

        public async Task Save<T>(T obj, string name)
        {
            Type type = obj.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());
            try
            {
                await Delete<T>(name);
                await WriteHeaders<T>(props);
                using (StreamWriter writer = File.AppendText(_filePath))
                {
                    using (var csvWriter = new CsvWriter(writer))
                    {
                        csvWriter.Configuration.Delimiter = _delimiter.ToString();
                        await Task.Run(() => csvWriter.WriteField(name));
                        foreach (PropertyInfo prop in props)
                        {
                            await Task.Run(() => csvWriter.WriteField(prop.GetValue(obj)));
                        }
                        csvWriter.NextRecord();
                    }
                }
            }
            catch (Exception)
            {
                //Silently ignoring
            }
        }

        public Task<string[]> GetAllRecordsIDs<T>()
        {
            if (!File.Exists(_filePath)) return Task.FromResult(new string[0]);
            try
            {
                int index;
                List<string> lines = File.ReadAllLines(_filePath).ToList();
                List<string> line = lines[0].Split(',').ToList();
                List<string> records = new List<string>();
                if (!line.Contains(_idname)) return Task.FromResult(new string[0]);
                index = line.IndexOf(_idname);
                foreach (string record in lines)
                {
                    line = record.Split(',').ToList();
                    if (line[index] != _idname)
                    {
                        records.Add(line[index].ToString());
                    }
                }
                return Task.FromResult(records.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Task WriteHeaders<T>(IList<PropertyInfo> props)
        {
            if (File.Exists(_filePath)) return Task.CompletedTask;
            try
            {
                using (StreamWriter file = File.AppendText(_filePath))
                {
                    using (var csvWriter = new CsvWriter(file))
                    {
                        csvWriter.Configuration.Delimiter = _delimiter.ToString();
                        csvWriter.WriteField(_idname);
                        foreach (PropertyInfo prop in props)
                        {
                            csvWriter.WriteField(prop.Name);
                        }
                        csvWriter.NextRecord();
                    }
                }
            }
            catch (Exception)
            {
                //Silently ignoring.
            }
            return Task.CompletedTask;
        }

        private async Task<string[]> ReadHeaders<T>()
        {
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.Delimiter = _delimiter.ToString();
                        await csv.ReadAsync();
                        csv.ReadHeader();
                        string[] headerRow = csv.Context.HeaderRecord;
                        return headerRow;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
