using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NoteViewModel: ViewModelBase
    {
        private readonly NoteService _noteService;
        private Note _note;
        private string _notePath;
        private string _noteTitle;
        private string _noteText;
        private string _createdString;
        private string _lastEditedString;

        public NoteViewModel (NoteService noteService)
        {
            _noteService = noteService;
            SaveNote = new Command(SaveNoteExecute, () => CanSave);
            DeleteNote = new Command(DeleteNoteExecute, () => CanDelete);
            UpdateNoteView();
        }

        public NoteViewModel(NoteService noteService, string path)
        {
            _noteService = noteService;
            SaveNote = new Command(SaveNoteExecute, () => CanSave);
            DeleteNote = new Command(DeleteNoteExecute, () => CanDelete);
            if (!String.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                _note = _noteService.OpenNote(path);
                _notePath = path;
            }
            else
            {
                _note = null;
            }
            UpdateNoteView();
        }

        #region Properties
        public string NoteTitleEntry
        {
            get
            { 
                return _noteTitle;
            }

            set
            {
                _noteTitle = value;
                OnPropertyChanged(nameof(NoteTitleEntry));
                SaveNote.ChangeCanExecute();
            }
        }

        public string NoteTextEditor
        {
            get
            {
                return _noteText;
            }

            set
            {
                _noteText = value;
                OnPropertyChanged(nameof(NoteTextEditor));
            }
        }

        public string CreatedString
        {
            get
            {
                return _createdString;
            }

            private set
            {
                _createdString = value;
                OnPropertyChanged(nameof(CreatedString));
            }
        }

        public string LastEditedString
        {
            get
            {
                return _lastEditedString;
            }

            private set
            {
                _lastEditedString = value;
                OnPropertyChanged(nameof(LastEditedString));
            }
        }

        public Command SaveNote { get; private set; }
        public Command DeleteNote { get; private set; }

        public bool CanSave => !String.IsNullOrWhiteSpace(NoteTitleEntry);
        public bool CanDelete => _note != null;

        #endregion

        void SaveNoteExecute()
        {
            if(_note != null)
            {
                _note.Title = NoteTitleEntry;
                _note.Text = NoteTextEditor;
                _note.LastEdited = DateTime.Now;
                _noteService.SaveWithDynamicFileName(_note, _notePath);
            }
            else
            {
                _note = new Note(NoteTitleEntry, NoteTextEditor, DateTime.Now, DateTime.Now);
                _notePath = _noteService.SaveWithDynamicFileName(_note);
            }
            UpdateNoteView();
            DeleteNote.ChangeCanExecute();
        }

        void DeleteNoteExecute()
        {
            _noteService.DeleteNoteFile(_notePath);
            UpdateNoteView();
            SaveNote.ChangeCanExecute();
            DeleteNote.ChangeCanExecute();
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void UpdateNoteView()
        {
            if (_note != null)
            {
                NoteTitleEntry = _note.Title;
                NoteTextEditor = _note.Text;
                CreatedString = _note.Created.ToString();
                LastEditedString = _note.LastEdited.ToString();
            }
            else
            {
                NoteTitleEntry = String.Empty;
                NoteTextEditor = String.Empty;
                CreatedString = String.Empty;
                LastEditedString = String.Empty;
            }
        }
    }
}