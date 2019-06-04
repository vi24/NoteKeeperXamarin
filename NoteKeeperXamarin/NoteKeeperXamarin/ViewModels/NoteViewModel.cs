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
                if(_noteOperator.Note != null)
                {
                    _noteOperator.Note.Title = _noteTitle;
                }
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
                if(_noteOperator.Note != null)
                {
                    _noteOperator.Note.Text = _noteText;
                }
                OnPropertyChanged(nameof(NoteTextEditor));
            }
        }

        public string CreatedString
        {
            get
            {
                if (_noteOperator.Note == null)
                {
                    _createdString = String.Empty;
                    return _createdString;
                }
                _createdString = _noteOperator.Note.Created.ToString();
                return _createdString;
            }

            set
            {
                _createdString = value;
                OnPropertyChanged(nameof(CreatedString));
            }
        }

        public string LastEditedString
        {
            get
            {
                if (_noteOperator.Note == null)
                {
                    _lastEditedString = String.Empty;
                    return _lastEditedString;
                }
                _lastEditedString = _noteOperator.Note.LastEdited.ToString();
                return _lastEditedString;
            }

            set
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
        }

        void DeleteNoteExecute()
        {
            NoteTitleEntry = String.Empty;
            NoteTextEditor = String.Empty;
            CreatedString = String.Empty;
            LastEditedString = String.Empty;
            _noteOperator.DeleteNote();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}