using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace NoteKeeperXamarin.Tests
{
    public class CSVServiceTest : IDisposable
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string CSV_EXTENSION = ".csv";

        public CSVServiceTest()
        {
            Directory.CreateDirectory(PATH);
        }

        public void Dispose()
        {
            DirectoryInfo dInfo = new DirectoryInfo(PATH);
            FileInfo[] files = dInfo.GetFiles("*" + CSV_EXTENSION)
                     .Where(p => p.Extension == CSV_EXTENSION).ToArray();
            foreach (FileInfo file in files)
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch { }
        }

        [Fact]
        public async void GivenCSVServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            string combinedPathToFile = Path.Combine(PATH, "test" + CSV_EXTENSION);
            CSVStorageService storageService = new CSVStorageService();
            DateTime dateTime = DateTime.UtcNow;
            Note expectedNote = new Note("Titel", "Foo", dateTime.ToString("o"), dateTime.ToString("o"));
            //Act
            await storageService.Save(expectedNote, combinedPathToFile);
            Note actualNote = await storageService.Open<Note>(combinedPathToFile);
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.LastEditedRoundTrip, actualNote.LastEditedRoundTrip);
            Assert.Equal(expectedNote.CreatedRoundTrip, actualNote.CreatedRoundTrip);
        }

        [Fact]
        public async void SaveDynamicFileName_GivenCSVServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            NoteService noteService = new NoteService(PATH, new CSVStorageService());
            Note note = new Note("Titel", "Foo", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            string path = await noteService.SaveNote(note);
            //DateTime created = ;
            note = await noteService.OpenNote(path);
            //Act
            await noteService.SaveNote(note, path);
            note = await noteService.OpenNote(path);
            DateTime lastedited = DateTime.Parse(note.LastEditedRoundTrip, null, System.Globalization.DateTimeStyles.RoundtripKind);
            DateTime created = DateTime.Parse(note.CreatedRoundTrip, null, System.Globalization.DateTimeStyles.RoundtripKind);
            //Assert
            Assert.True(lastedited > created);
        }

        [Fact]
        public void SaveToFile_GivenCSVServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            CSVStorageService storageService = new CSVStorageService();
            Note note = new Note("Titel", "Foo", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            Assert.ThrowsAsync<DirectoryNotFoundException>(() => storageService.Save<Note>(note, @"C:\NotExistingPath\A"));
        }
    }
}
