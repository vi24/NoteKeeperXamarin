using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace NoteKeeperXamarin.Tests
{
    public class SingleCSVStorageServiceTest: IDisposable
    {
        private readonly SingleCSVStorageService singleCSVStorageService;
        private readonly string PATH = @"C:\GitHub\NoteKeeperXamarin\SerializedNotesTest";
        private readonly string FILE_PATH;

        public SingleCSVStorageServiceTest()
        {
            Directory.CreateDirectory(PATH);
            singleCSVStorageService = new SingleCSVStorageService(PATH);
            FILE_PATH = Path.Combine(PATH, "notes" + singleCSVStorageService.FileExtensionName);
            File.Delete(FILE_PATH);
        }
        public void Dispose()
        {
            File.Delete(FILE_PATH);
        }

        [Fact]
        public async void GivenTwoNotes_WhenWritingToCSV_ThenThereShouldBeTwoRecords()
        {
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooTextWW", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            int lines = File.ReadAllLines(FILE_PATH).Count();
            Assert.True( lines == 2);
        }

        [Fact]
        public async void GivenANote_WhenOpeningThatNote_ThenTheContentShouldBeTheSame()
        {
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            Note expected = new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            Note actual = await singleCSVStorageService.Open<Note>("hello");
            Assert.Equal(expected.Title, actual.Title);
        }

        [Fact]
        public async void GivenTwoNotes_WhenDeletingANote_ThenThereShouldBeTwoRecords()
        {
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooText", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "fwww");
            await singleCSVStorageService.Save<Note>(new Note("fooTitle", "fooTextWW", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o")), "hello");
            await singleCSVStorageService.Delete<Note>("hello");
            int lines = File.ReadAllLines(FILE_PATH).Count();
            Assert.True(lines == 2);
        }

        
    }
}
