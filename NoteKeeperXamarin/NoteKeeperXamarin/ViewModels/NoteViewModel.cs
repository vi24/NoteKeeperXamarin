using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Operator;
using NoteKeeperXamarin.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NoteViewModel: INotifyPropertyChanged
    {
        private NoteOperator _noteOperator;
        private string _noteTitle;
        private string _noteText;
        private string _createdString;
        private string _lastEditedString;

        public NoteViewModel(IStorageService service)
        {
            _noteOperator = new NoteOperator(service);
            SaveNote = new Command(SaveNoteExecute, () => CanSave);
            DeleteNote = new Command(DeleteNoteExecute, () => _noteOperator.Note != null);
            UpdateNoteView();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public bool CanSave => !String.IsNullOrWhiteSpace(NoteTitleEntry);
        public Command SaveNote { get; private set; }
        public Command DeleteNote { get; private set; }

        void SaveNoteExecute()
        {
            _noteOperator.SaveWithStaticFileName(NoteTitleEntry, NoteTextEditor);
            UpdateNoteView();
            DeleteNote.ChangeCanExecute();
        }

        void DeleteNoteExecute()
        {
            _noteOperator.DeleteNote();
            UpdateNoteView();
            SaveNote.ChangeCanExecute();
            DeleteNote.ChangeCanExecute();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateNoteView()
        {
            if (_noteOperator.Note != null)
            {
                NoteTitleEntry = _noteOperator.Note.Title;
                NoteTextEditor = _noteOperator.Note.Text;
                CreatedString = _noteOperator.Note.Created.ToString();
                LastEditedString = _noteOperator.Note.LastEdited.ToString();
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