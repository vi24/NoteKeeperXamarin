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
        private readonly string PATH = @"C:\GitHub\NoteKeeperXamarin\SerializedNotes";

        [Fact]
        public async void GivenTwoNotes_WhenWritingToCSV_ThenThereShouldBeTwoRecords()
        {
            
            SingleCSVStorageService singleCSVStorageService = new SingleCSVStorageService();
            string path = Path.Combine(PATH, "notes" + singleCSVStorageService.FileExtensionName);
            File.Delete(path);
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooTextWW", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            Assert.True(File.ReadAllLines(path).Count() == 2);
        }

        //[Fact]
        //public async void ReadHeaderTest()
        //{
        //    string[] expectedRows = new string []{ "UniqueName", "Text", "Title", "CreatedRoundTrip", "LastEditedRoundTrip" };
        //    SingleCSVStorageService singleCSVStorageService = new SingleCSVStorageService();
        //    string path = Path.Combine(PATH, "notes" + singleCSVStorageService.FileExtensionName);
        //    string[] actualRows = await singleCSVStorageService.ReadHeaders<Note>();
        //    Assert.Equal(expectedRows, actualRows);
        //}

        [Fact]
        public async void OpenTest()
        {
            SingleCSVStorageService singleCSVStorageService = new SingleCSVStorageService();
            string path = Path.Combine(PATH, "notes" + singleCSVStorageService.FileExtensionName);
            Note expected = new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            Note actual = await singleCSVStorageService.Open<Note>("hello");
            Assert.Equal(expected.Title, actual.Title);
        }

        [Fact]
        public async void DeleteTest()
        {
            SingleCSVStorageService singleCSVStorageService = new SingleCSVStorageService();
            string path = Path.Combine(PATH, "notes" + singleCSVStorageService.FileExtensionName);
            File.Delete(path);
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "fwww");
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooTextWW", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            await singleCSVStorageService.Delete<Note>("hello");
            Assert.True(File.ReadAllLines(path).Count() == 2);
        }
    }
}
