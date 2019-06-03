using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Operator;
using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;
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
        public void GivenXMLServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            SetUp();
            XMLStorageService storageService = new XMLStorageService();
            Note expectedNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            //Act
            storageService.SaveToFile<Note>(expectedNote, Path.Combine(PATH, "test" + XML_EXTENSION));
            Note actualNote = (Note)storageService.OpenFile<Note>(Path.Combine(PATH, "test" + XML_EXTENSION));
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            TearDown();
        }

        [Fact]
        public void SaveWithStaticFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenCurrentLastEditedTimeShouldBeGreaterThanPreviousLastEditedTime()
        {
            //Arrange
            SetUp();
            NoteOperator noteOperator = new NoteOperator(new XMLStorageService(), PATH);
            noteOperator.SaveWithStaticFileName("Titel", "Foo");
            long previousLastEditedFileTime = noteOperator.Note.LastEdited.ToFileTime();
            noteOperator.OpenLastSavedNote();
            //Act
            noteOperator.SaveWithStaticFileName("Titel", "Foo");
            long currentLastEditedFileTime = noteOperator.Note.LastEdited.ToFileTime();
            //Assert
            Assert.True(currentLastEditedFileTime > previousLastEditedFileTime);
            TearDown();
        }

        [Fact]
        public void SaveWithStaticFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            SetUp();
            NoteOperator noteOperator = new NoteOperator(new XMLStorageService(), PATH);
            noteOperator.SaveWithStaticFileName("Titel", "Foo");
            long createdFileTime = noteOperator.Note.Created.ToFileTime();
            noteOperator.OpenLastSavedNote();
            //Act
            noteOperator.SaveWithStaticFileName("Titel", "Foo");
            long lastEditedFileTime = noteOperator.Note.LastEdited.ToFileTime();
            //Assert
            Assert.True(lastEditedFileTime > createdFileTime);
            TearDown();
        }

        [Fact]
        public void SaveWithStaticFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenCreatedTimeShouldStayTheSame()
        {
            //Arrange
            SetUp();
            NoteOperator noteOperator = new NoteOperator(new XMLStorageService(), PATH);
            noteOperator.SaveWithStaticFileName("Titel", "Foo");
            long expectedDateTime = noteOperator.Note.Created.ToFileTime();
            //Act
            noteOperator.SaveWithStaticFileName("Titel", "Foo");
            long actualDateTime = noteOperator.Note.Created.ToFileTime();
            //Assert
            Assert.Equal(expectedDateTime, actualDateTime);
            TearDown();
        }

        [Fact]
        public void SaveToFile_GivenXMLServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            XMLStorageService storageService = new XMLStorageService();
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            Assert.Throws<DirectoryNotFoundException>(() => storageService.SaveToFile<Note>(note, @"C:\NotExistingPath\A"));
        }

        [Fact]
        public void GivenJSONServiceAndNote_WhenSavingFileAndClosingApp_ThenTheSameNoteShouldBeLoadedViaMetaDataFileNextTimeAfterOpeningApp()
        {
            SetUp();
            NoteOperator noteOperator = new NoteOperator(new XMLStorageService(), PATH);
            noteOperator.SaveWithDynamicFileName("Titel", "Foo");
            Note expectedNote = noteOperator.Note;
            noteOperator = new NoteOperator(new XMLStorageService(), PATH);
            noteOperator.OpenLastSavedNoteViaMetaData();
            Note actualNote = noteOperator.Note;
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            TearDown();
        }
    }
}
