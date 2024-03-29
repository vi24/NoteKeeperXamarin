﻿using Moq;
using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NoteKeeperXamarin.Tests
{
    public class NoteViewModelTest
    {

        [Fact]
        public void GivenNote_WhenOpeningNote_ThenNoteTitleAndCreatedAndLastEditedShouldBeSetInViewAsync()
        {
            //Arrange
            string fooTitle = "FooTitle";
            string fooText = "FooText";
            Note note = new Note(fooTitle, fooText, DateTime.UtcNow.ToString("o"), DateTime.UtcNow.ToString("o"));
            var mock = new Mock<IStorageService>();
            mock.Setup(x => x.Open<Note>(It.IsAny<string>())).Returns(Task.FromResult(note));
            NoteService noteService = new NoteService(String.Empty, mock.Object);
            //Act
            NoteViewModel noteVM = new NoteViewModel(noteService, "foo");
            //Assert
            Assert.Equal(noteVM.NoteTitleEntry, fooTitle);
            //TearDown
        }

        [Fact]
        public void GivenEmptyNote_WhenSavingNoteWithTitleAndText_ThenCreatedAndLastEditedShouldBeSetInView()
        {
            //Arrange
            string fooTitle = "FooTitle";
            string fooText = "FooText";
            var mock = new Mock<IStorageService>();
            NoteService noteService = new NoteService("", mock.Object);
            NoteViewModel noteVM = new NoteViewModel(noteService);
            noteVM.NoteTitleEntry = fooTitle;
            noteVM.NoteTextEditor = fooText;
            //Act
            noteVM.SaveNote.Execute();
            //Assert
            Assert.NotNull(noteVM.CreatedString);
            Assert.NotNull(noteVM.LastEditedString);
            Assert.Equal(noteVM.CreatedString, noteVM.LastEditedString);
        }

        [Fact]
        public void GivenNote_WhenOverridingNoteWithTitleAndText_ThenCreatedShouldBeNotEqualToLastEditedInView()
        {
            //Arrange
            string fooTitle = "FooTitle";
            string fooText = "FooText";
            var mock = new Mock<IStorageService>();
            NoteService noteService = new NoteService("", mock.Object);
            NoteViewModel noteVM = new NoteViewModel(noteService);
            noteVM.NoteTitleEntry = fooTitle;
            noteVM.NoteTextEditor = fooText;
            //Act
            noteVM.SaveNote.Execute();
            Thread.Sleep(1000);
            noteVM.SaveNote.Execute();
            //Assert
            Assert.NotNull(noteVM.CreatedString);
            Assert.NotNull(noteVM.LastEditedString);
            Assert.NotEqual(noteVM.CreatedString, noteVM.LastEditedString);
        }
    }
}
