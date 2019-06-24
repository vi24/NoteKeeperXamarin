using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace NoteKeeperChallenge.Tests
{
    public class XMLServiceTest : IDisposable
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string XML_EXTENSION = ".xml";

        public XMLServiceTest()
        {
            Directory.CreateDirectory(PATH);
        }

        public void Dispose()
        {
            DirectoryInfo dInfo = new DirectoryInfo(PATH);
            FileInfo[] files = dInfo.GetFiles("*" + XML_EXTENSION)
                     .Where(p => p.Extension == XML_EXTENSION).ToArray();
            foreach (FileInfo file in files)
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch { }
        }

        [Fact]
        public async void GivenXMLServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
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
        }

        [Fact]
        public async void SaveDynamicFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
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
