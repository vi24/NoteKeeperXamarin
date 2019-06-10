using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace NoteKeeperXamarin.Tests
{
    public class NoteViewModelTest
    {

        private readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\SerializedNotesTest");
        private const string JSON_EXTENSION = ".json";

        private void SetUp()
        {
            Directory.CreateDirectory(PATH);
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

        //[Fact]
        //public void SaveWithStaticFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenCreatedTimeShouldStayTheSame()
        //{
        //    //Arrange
        //    SetUp();
        //    NoteViewModel noteVM = new NoteViewModel(new NoteService(new JSONStorageService(), PATH));
        //    Note expectedNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
        //    noteVM.SaveNote.Execute();
        //    string expectedCreatedTimeInString = expectedNote.Created.ToString();
        //    //Act
        //    noteVM.SaveNote.Execute(expectedNote);
        //    string actualCreatedTimeInString = noteVM.CreatedString;
        //    //Assert
        //    Assert.Equal(expectedCreatedTimeInString, actualCreatedTimeInString);
        //    TearDown();
        //}

        [Fact]
        public void SaveWithStaticFileName_GivenJSONServiceTitleAndText_WhenOverridingOldFile_ThenCurrentLastEditedTimeShouldBeGreaterThanPreviousLastEditedTime()
        {
            //Arrange
            SetUp();
            Note previousNote = new Note("Titel", "Foo", DateTime.Now, DateTime.Now);
            NoteService noteService = new NoteService(new JSONStorageService(), PATH);
            noteService.SaveWithStaticFileName(previousNote);
            long previousLastEditedFileTime = previousNote.LastEdited.ToFileTime();
            Note currentNote = noteService.OpenLastSavedNote();
            //Act
            noteService.SaveWithStaticFileName(currentNote);
            long currentLastEditedFileTime = currentNote.LastEdited.ToFileTime();
            //Assert
            Assert.True(currentLastEditedFileTime > previousLastEditedFileTime);
            TearDown();
        }
    }
}
