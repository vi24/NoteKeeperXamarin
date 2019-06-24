using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using Splat;
using System;
using System.IO;
using Xunit;

namespace NoteKeeperChallenge.Tests
{
    public class XMLServiceTest
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string XML_EXTENSION = ".xml";

        private void SetUp()
        {
            Directory.CreateDirectory(PATH);
        }

        private void TearDown()
        {
            DirectoryInfo dInfo = new DirectoryInfo(PATH);
            foreach (FileInfo file in dInfo.GetFiles())
            {
                if (!(Path.GetExtension(file.FullName) == XML_EXTENSION)) return;
                file.Delete();
            }
            Directory.Delete(PATH);
        }

        [Fact]
        public async void GivenXMLServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            SetUp();
            string combinedPathToFile = Path.Combine(PATH, "test" + XML_EXTENSION);
            XMLStorageService storageService = new XMLStorageService();
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
            TearDown();
        }

        [Fact]
        public async void SaveDynamicFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            SetUp();
            NoteService noteService = new NoteService(PATH, new XMLStorageService());
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
            TearDown();
        }

        [Fact]
        public void SaveToFile_GivenXMLServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            XMLStorageService storageService = new XMLStorageService();
            Note note = new Note("Titel", "Foo", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            Assert.ThrowsAsync<DirectoryNotFoundException>(() => storageService.Save<Note>(note, @"C:\NotExistingPath\A"));
        }
    }
}
