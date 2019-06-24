using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NoteKeeperChallenge.Tests
{
    public class JSONServiceTest : IDisposable
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string JSON_EXTENSION = ".json";

        public JSONServiceTest()
        {
            Directory.CreateDirectory(PATH);
        }

        public void Dispose()
        {
            DirectoryInfo dInfo = new DirectoryInfo(PATH);
            FileInfo[] files = dInfo.GetFiles("*" + JSON_EXTENSION)
                     .Where(p => p.Extension == JSON_EXTENSION).ToArray();
            foreach (FileInfo file in files)
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch { }
        }

        [Fact]
        public async void GivenJSONServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            JSONStorageService storageService = new JSONStorageService();
            Note expectedNote = new Note("Titel", "Foo", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            //Act
            await storageService.Save(expectedNote, Path.Combine(PATH, "test" + JSON_EXTENSION));
            Note actualNote = await storageService.Open<Note>(Path.Combine(PATH, "test" + JSON_EXTENSION));
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.True(expectedNote.LastEditedRoundTrip == actualNote.LastEditedRoundTrip);
            Assert.True(expectedNote.CreatedRoundTrip == actualNote.CreatedRoundTrip);
        }

        [Fact]
        public async void SaveDynamicFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            NoteService noteService = new NoteService(PATH, new JSONStorageService());
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
        public async Task SaveToFile_GivenJSONServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            JSONStorageService storageService = new JSONStorageService();
            Note note = new Note("Titel", "Foo", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => storageService.Save<Note>(note, @"C:\NotExistingPath\A"));
        }
    }
}
