﻿//using NoteKeeperXamarin.Models;
//using NoteKeeperXamarin.Services;
//using Splat;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using Xunit;

//namespace NoteKeeperXamarin.Tests
//{
//    public class CSVServiceTest
//    {
//        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
//        private const string CSV_EXTENSION = ".csv";

//        private void SetUp()
//        {
//            Directory.CreateDirectory(PATH);
//            Locator.CurrentMutable.Register(() => new CSVStorageService(), typeof(IStorageService));
//        }

//        private void TearDown()
//        {
//            DirectoryInfo dInfo = new DirectoryInfo(PATH);
//            foreach (FileInfo file in dInfo.GetFiles())
//            {
//                if (!(Path.GetExtension(file.FullName) == CSV_EXTENSION)) return;
//                file.Delete();
//            }
//            Directory.Delete(PATH);
//        }

//        [Fact]
//        public async void GivenCSVServiceTitleAndText_WhenSavingNewFileAndReadingOut_ThenTheContentShouldBeTheSame()
//        {
//            //Arrange
//            SetUp();
//            string combinedPathToFile = Path.Combine(PATH, "test" + CSV_EXTENSION);
//            CSVStorageService storageService = new CSVStorageService();
//            DateTime dateTime = DateTime.UtcNow;
//            Note expectedNote = new Note("Titel", "Foo", dateTime.ToString("o"), dateTime.ToString("o"));
//            //Act
//            await storageService.Save(expectedNote, combinedPathToFile);
//            Note actualNote = await storageService.Open<Note>(combinedPathToFile);
//            //Assert
//            Assert.Equal(expectedNote.Title, actualNote.Title);
//            Assert.Equal(expectedNote.Text, actualNote.Text);
//            Assert.Equal(expectedNote.LastEditedRoundTrip.ToString(), actualNote.LastEditedRoundTrip.ToString());
//            Assert.Equal(expectedNote.CreatedRoundTrip.ToString(), actualNote.CreatedRoundTrip.ToString());
//            TearDown();
//        }

//        [Fact]
//        public async void SaveDynamicFileName_GivenCSVServiceTitleAndText_WhenOverridingOldFile_ThenLastEditedTimeShouldBeGreaterThanCreatedTime()
//        {
//            //Arrange
//            SetUp();
//            NoteService noteService = new NoteService(PATH);
//            Note note = new Note("Titel", "Foo", DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
//            string path = await noteService.SaveNote(note);
//            //DateTime created = ;
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
//        public void SaveToFile_GivenCSVServiceAndNonExistingPath_WhenSavingFile_ThenItShouldThrowDirectoryNotFoundException()
//        {
//            CSVStorageService storageService = new CSVStorageService();
//            Note note = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
//            Assert.ThrowsAsync<DirectoryNotFoundException>(() => storageService.Save<Note>(note, @"C:\NotExistingPath\A"));
//        }
//    }
//}
