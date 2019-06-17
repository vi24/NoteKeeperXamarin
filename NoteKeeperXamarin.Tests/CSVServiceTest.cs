using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace NoteKeeperXamarin.Tests
{
    public class CSVServiceTest
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string CSV_EXTENSION = ".csv";

        private void SetUp()
        {
            Directory.CreateDirectory(PATH);
            Locator.CurrentMutable.Register(() => new CSVStorageService(), typeof(IStorageService));
        }

        private void TearDown()
        {
            DirectoryInfo dInfo = new DirectoryInfo(PATH);
            foreach (FileInfo file in dInfo.GetFiles())
            {
                if (!(Path.GetExtension(file.FullName) == CSV_EXTENSION)) return;
                file.Delete();
            }
            Directory.Delete(PATH);
        }

        [Fact]
        public void GivenCSVServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            SetUp();
            CSVStorageService storageService = new CSVStorageService();
            Note expectedNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            //Act
            storageService.SaveToFile(expectedNote, Path.Combine(PATH, "test" + CSV_EXTENSION));
            Note actualNote = storageService.OpenFile<Note>(Path.Combine(PATH, "test" + CSV_EXTENSION));
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            TearDown();
        }

        [Fact]
        public void SaveDynamicFileName_GivenCSVServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
            //Arrange
            SetUp();
            NoteService noteService = new NoteService(PATH);
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            string path = noteService.SaveWithDynamicFileName(note);
            long createdFileTime = note.Created.ToFileTime();
            note = noteService.OpenNote(path);
            //Act
            noteService.SaveWithDynamicFileName(note, path);
            note = noteService.OpenNote(path);
            long lastEditedFileTime = note.LastEdited.ToFileTime();
            //Assert
            Assert.True(lastEditedFileTime > createdFileTime);
            TearDown();
        }

        [Fact]
        public void SaveToFile_GivenCSVServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            CSVStorageService storageService = new CSVStorageService();
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            Assert.Throws<DirectoryNotFoundException>(() => storageService.SaveToFile<Note>(note, @"C:\NotExistingPath\A"));
        }
    }
}
