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
        private bool _canSave;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NoteTitleEntry { get; set; }
        public string NoteTextEditor { get; set; }
        public string CreatedString
        {
            get
            {
                return _noteOperator.Note.Created.ToString();
            }
        }
        public string LastEditedDateTime
        {
            get
            {
                return _noteOperator.Note.LastEdited.ToString();
            }
        }
        public bool CanSave
        {
            get
            {
                if (String.IsNullOrWhiteSpace(NoteTitleEntry)) return false;
                return true;
            }
            set
            {
                _canSave = value;
                OnPropertyChanged("CanSave");
            }
        }

        public Note Note { get; private set; }
        public MetaData MetaData { get; private set; }

        public ICommand SaveNote { get; private set; }
        public ICommand DeleteNote { get; private set; }

        public NoteViewModel(IStorageService service)
        {
            _noteOperator = new NoteOperator(service);
            SaveNote = new Command(SaveNoteExecute, SaveNoteCanExecute);
            NoteTitleEntry = "Hallo";
            NoteTextEditor = "yes";
        }

        void SaveNoteExecute()
        {
            _noteOperator.SaveWithStaticFileName(NoteTitleEntry, NoteTextEditor);
        }

        bool SaveNoteCanExecute()
        {
            return CanSave;
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