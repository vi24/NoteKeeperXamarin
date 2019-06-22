using CsvHelper;
using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace NoteKeeperXamarin.Tests
{
    public class SingleCSVStorageServiceTest
    {
        private readonly string PATH = @"C:\GitHub\NoteKeeperXamarin\SerializedNotesTest";

        [Fact]
        public async void GivenTwoNotes_WhenWritingToCSV_ThenThereShouldBeTwoRecords()
        {
            List<Note> noteList;
            SingleCSVStorageService singleCSVStorageService = new SingleCSVStorageService();
            string path = Path.Combine(PATH, "noteTest" + singleCSVStorageService.FileExtensionName);
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), path);
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooTextWW", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), path);
            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.HasHeaderRecord = false;
                    noteList = csv.GetRecords<Note>().ToList();
                }
            }
            Assert.True(noteList.Count == 2);
        }

    }
}
