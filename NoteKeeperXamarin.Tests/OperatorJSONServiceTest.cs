using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;
using System;
using System.IO;
using Xunit;

namespace NoteKeeperChallenge.Tests
{
    public class OperatorJSONServiceTest
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotes");
        private const string JSON_EXTENSION = ".json";
        
        [Fact]
        public void GivenJSONServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            JSONStorageService storageService = new JSONStorageService();
            Note expectedNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            //Act
            storageService.SaveToFile(expectedNote, Path.Combine(PATH, "test" + JSON_EXTENSION));
            Note actualNote = storageService.OpenFile<Note>(Path.Combine(PATH,"test" + JSON_EXTENSION));
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
        }

        [Fact]
        public void SaveWithStaticFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenCurrentLastEditedTimeShouldBeGreaterThanPreviousLastEditedTime()
        {
            //Arrange
            NoteViewModel noteViewModel = new NoteViewModel(new JSONStorageService(), PATH);
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long previousLastEditedFileTime = noteViewModel.Note.LastEdited.ToFileTime();
            noteViewModel.OpenLastSavedNote();
            //Act
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long currentLastEditedFileTime = noteViewModel.Note.LastEdited.ToFileTime();
            //Assert
            Assert.True(currentLastEditedFileTime > previousLastEditedFileTime);
        }

        [Fact]
        public void SaveWithStaticFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            NoteViewModel noteViewModel = new NoteViewModel(new JSONStorageService(), PATH);
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long createdFileTime = noteViewModel.Note.Created.ToFileTime();
            noteViewModel.OpenLastSavedNote();
            //Act
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long lastEditedFileTime = noteViewModel.Note.LastEdited.ToFileTime();
            //Assert
            Assert.True(lastEditedFileTime > createdFileTime);
        }

        [Fact]
        public void SaveWithStaticFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenCreatedTimeShouldStayTheSame()
        {
            //Arrange
            NoteViewModel noteViewModel = new NoteViewModel(new JSONStorageService(), PATH);
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long expectedDateTime = noteViewModel.Note.Created.ToFileTime();
            //Act
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long actualDateTime = noteViewModel.Note.Created.ToFileTime();
            //Assert
            Assert.Equal(expectedDateTime, actualDateTime);
        }

        [Fact]
        public void SaveToFile_GivenJSONServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            JSONStorageService storageService = new JSONStorageService();
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            Assert.Throws<DirectoryNotFoundException>(() => storageService.SaveToFile<Note>(note, @"C:\NotExistingPath\A"));
        }

        [Fact]
        public void GivenJSONServiceAndNote_WhenSavingFileAndClosingApp_ThenTheSameNoteShouldBeLoadedViaMetaDataFileNextTimeAfterOpeningApp()
        {
            NoteViewModel noteViewModel = new NoteViewModel(new JSONStorageService(), PATH);
            noteViewModel.SaveWithDynamicFileName("Titel", "Foo");
            Note expectedNote = noteViewModel.Note;
            noteViewModel = new NoteViewModel(new JSONStorageService(), PATH);
            noteViewModel.OpenLastSavedNoteViaMetaData();
            Note actualNote = noteViewModel.Note;
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
        }
    }
}
