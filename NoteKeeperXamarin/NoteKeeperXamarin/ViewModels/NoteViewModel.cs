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

        public event PropertyChangedEventHandler PropertyChanged;

        public string NoteTitleEntry { get; set; }
        public string NoteTextEditor { get; set; }
        public DateTime CreatedLabel { get; set; }
        public DateTime LastEditedLabel { get; set; }
        public bool CanSave
        {
            get
            {
                if (String.IsNullOrWhiteSpace(NoteTitleEntry)) return false;
                return true;
            }
            set
            {   

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




        
    }
}