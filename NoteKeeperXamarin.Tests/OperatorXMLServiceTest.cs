﻿using NoteKeeperXamarin.Models;
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
            NoteViewModel noteViewModel = new NoteViewModel(new XMLStorageService(), PATH);
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long previousLastEditedFileTime = noteViewModel.Note.LastEdited.ToFileTime();
            noteViewModel.OpenLastSavedNote();
            //Act
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long currentLastEditedFileTime = noteViewModel.Note.LastEdited.ToFileTime();
            //Assert
            Assert.True(currentLastEditedFileTime > previousLastEditedFileTime);
            TearDown();
        }

        [Fact]
        public void SaveWithStaticFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            SetUp();
            NoteViewModel noteViewModel = new NoteViewModel(new XMLStorageService(), PATH);
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long createdFileTime = noteViewModel.Note.Created.ToFileTime();
            noteViewModel.OpenLastSavedNote();
            //Act
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long lastEditedFileTime = noteViewModel.Note.LastEdited.ToFileTime();
            //Assert
            Assert.True(lastEditedFileTime > createdFileTime);
            TearDown();
        }

        [Fact]
        public void SaveWithStaticFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenCreatedTimeShouldStayTheSame()
        {
            //Arrange
            SetUp();
            NoteViewModel noteViewModel = new NoteViewModel(new XMLStorageService(), PATH);
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long expectedDateTime = noteViewModel.Note.Created.ToFileTime();
            //Act
            noteViewModel.SaveWithStaticFileName("Titel", "Foo");
            long actualDateTime = noteViewModel.Note.Created.ToFileTime();
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
            NoteViewModel noteViewModel = new NoteViewModel(new XMLStorageService(), PATH);
            noteViewModel.SaveWithDynamicFileName("Titel", "Foo");
            Note expectedNote = noteViewModel.Note;
            noteViewModel = new NoteViewModel(new XMLStorageService(), PATH);
            noteViewModel.OpenLastSavedNoteViaMetaData();
            Note actualNote = noteViewModel.Note;
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            TearDown();
        }
    }
}
