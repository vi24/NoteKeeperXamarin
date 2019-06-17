using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using Splat;
using System;
using System.IO;
using Xunit;

namespace NoteKeeperChallenge.Tests
{
    public class JSONServiceTest
    {
        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string JSON_EXTENSION = ".json";

        private void SetUp()
        {
            Directory.CreateDirectory(PATH);
            Locator.CurrentMutable.Register(() => new JSONStorageService(), typeof(IStorageService));
        }

        private void TearDown()
        {
            DirectoryInfo dInfo = new DirectoryInfo(PATH);
            foreach (FileInfo file in dInfo.GetFiles())
            {
                if (!(Path.GetExtension(file.FullName) == JSON_EXTENSION)) return;
                file.Delete();
            }
            Directory.Delete(PATH);
        }
        
        [Fact]
        public void GivenJSONServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
        {
            //Arrange
            SetUp();
            IStorageService storageService = Locator.Current.GetService<IStorageService>();
            Note expectedNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            //Act
            storageService.SaveToFile(expectedNote, Path.Combine(PATH, "test" + JSON_EXTENSION));
            Note actualNote = storageService.OpenFile<Note>(Path.Combine(PATH,"test" + JSON_EXTENSION));
            //Assert
            Assert.Equal(expectedNote.Title, actualNote.Title);
            Assert.Equal(expectedNote.Text, actualNote.Text);
            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
            TearDown();
        }

        [Fact]
        public void SaveDynamicFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
        {
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
        public void SaveToFile_GivenJSONServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
        {
            IStorageService storageService = Locator.Current.GetService<IStorageService>();
            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            Assert.Throws<DirectoryNotFoundException>(() => storageService.SaveToFile<Note>(note, @"C:\NotExistingPath\A"));
        }
    }
}
