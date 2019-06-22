//using NoteKeeperXamarin.Models;
//using NoteKeeperXamarin.Services;
//using Splat;
//using System;
//using System.IO;
//using Xunit;

//namespace NoteKeeperChallenge.Tests
//{
//    public class XMLServiceTest
//    {
//        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
//        private const string XML_EXTENSION = ".xml";

//        private void SetUp()
//        {
//            Directory.CreateDirectory(PATH);
//            Locator.CurrentMutable.Register(() => new XMLStorageService(), typeof(IStorageService));
//        }

//        private void TearDown()
//        {
//            DirectoryInfo dInfo = new DirectoryInfo(PATH);
//            foreach (FileInfo file in dInfo.GetFiles())
//            {
//                if (!(Path.GetExtension(file.FullName) == XML_EXTENSION)) return;
//                file.Delete();
//            }
//            Directory.Delete(PATH);
//        }

//        [Fact]
//        public async void GivenXMLServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
//        {
//            //Arrange
//            SetUp();
//            IStorageService storageService = Locator.Current.GetService<IStorageService>();
//            Note expectedNote = new Note("Titel", "Foo", DateTime.UtcNow, DateTime.UtcNow);
//            //Act
//            await storageService.Save(expectedNote, Path.Combine(PATH, "test" + XML_EXTENSION));
//            Note actualNote = await storageService.Open<Note>(Path.Combine(PATH, "test" + XML_EXTENSION));
//            //Assert
//            Assert.Equal(expectedNote.Title, actualNote.Title);
//            Assert.Equal(expectedNote.Text, actualNote.Text);
//            Assert.Equal(expectedNote.LastEdited.ToString(), actualNote.LastEdited.ToString());
//            Assert.Equal(expectedNote.Created.ToString(), actualNote.Created.ToString());
//            TearDown();
//        }

//        [Fact]
//        public async void SaveDynamicFileName_GivenXMLServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
//        {
//            //Arrange
//            SetUp();
//            NoteService noteService = new NoteService(PATH);
//            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
//            string path = await noteService.SaveNote(note);
//            long createdFileTime = note.Created.ToFileTime();
//            note = await noteService.OpenNote(path);
//            //Act
//            await noteService.SaveNote(note, path);
//            note = await noteService.OpenNote(path);
//            long lastEditedFileTime = note.LastEdited.ToFileTime();
//            //Assert
//            Assert.True(lastEditedFileTime > createdFileTime);
//            TearDown();
//        }

//        [Fact]
//        public void SaveToFile_GivenXMLServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
//        {
//            IStorageService storageService = Locator.Current.GetService<IStorageService>();
//            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
//            Assert.ThrowsAsync<DirectoryNotFoundException>(() => storageService.Save<Note>(note, @"C:\NotExistingPath\A"));
//        }
//    }
//}
