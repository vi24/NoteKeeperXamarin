using NoteKeeperXamarin.Services;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NoteViewModel: INotifyPropertyChanged
    {
        private readonly NoteService _noteService;
        private string _noteTitle;
        private string _noteText;
        private string _createdString;
        private string _lastEditedString;

        public event PropertyChangedEventHandler PropertyChanged;

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
            if (!String.IsNullOrWhiteSpace(path))
            {
                _noteService.OpenNote(path);
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
        public bool CanDelete => _noteService.Note != null;

        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void SaveNoteExecute()
        {
            _noteService.SaveWithDynamicFileName(NoteTitleEntry, NoteTextEditor);
            UpdateNoteView();
            DeleteNote.ChangeCanExecute();
        }

        void DeleteNoteExecute()
        {
            _noteService.DeleteNoteFile();
            _noteService.Note = null;
            UpdateNoteView();
            SaveNote.ChangeCanExecute();
            DeleteNote.ChangeCanExecute();
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void UpdateNoteView()
        {
            if (_noteService.Note != null)
            {
                NoteTitleEntry = _noteService.Note.Title;
                NoteTextEditor = _noteService.Note.Text;
                CreatedString = _noteService.Note.Created.ToString();
                LastEditedString = _noteService.Note.LastEdited.ToString();
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