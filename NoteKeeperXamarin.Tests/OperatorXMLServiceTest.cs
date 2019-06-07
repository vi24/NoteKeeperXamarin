using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.IO;
using Xunit;

namespace NoteKeeperChallenge.Tests
{
    public class OperatorXMLServiceTest
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
        public void GivenJSONServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            SetUp();
            XMLStorageService storageService = new XMLStorageService();
            Note expectedNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            //Act
            storageService.SaveToFile(expectedNote, Path.Combine(PATH, "test" + XML_EXTENSION));
            Note actualNote = storageService.OpenFile<Note>(Path.Combine(PATH, "test" + XML_EXTENSION));
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            TearDown();
        }

        [Fact]
        public void SaveWithStaticFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            SetUp();
            NoteService noteService = new NoteService(new XMLStorageService(), PATH);
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            noteService.SaveWithStaticFileName(note);
            long createdFileTime = note.Created.ToFileTime();
            noteService.OpenLastSavedNote();
            //Act
            noteService.SaveWithStaticFileName(note);
            long lastEditedFileTime = note.LastEdited.ToFileTime();
            //Assert
            Assert.True(lastEditedFileTime > createdFileTime);
            TearDown();
        }

        [Fact]
        public void SaveToFile_GivenJSONServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            XMLStorageService storageService = new XMLStorageService();
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            Assert.Throws<DirectoryNotFoundException>(() => storageService.SaveToFile<Note>(note, @"C:\NotExistingPath\A"));
        }
    }
}
