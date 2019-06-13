using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NoteViewModel : ReactiveObject
    {
        private readonly NoteService _noteService;
        private Note _note;
        private string _notePath;
        private string _noteTitle;
        private string _noteText;
        private string _createdString;
        private string _lastEditedString;
        private bool _canDelete;

        public NoteViewModel(NoteService noteService)
        {
            _noteService = noteService;
            SaveNote = ReactiveCommand.Create(SaveNoteExecute, CanExecuteSave);
            DeleteNote = ReactiveCommand.Create(DeleteNoteExecute, CanExecuteDelete);
            SaveNote.Subscribe();
            DeleteNote.Subscribe();
            UpdateNoteView();
        }

        public NoteViewModel(NoteService noteService, string path)
        {
            _noteService = noteService;
            SaveNote = ReactiveCommand.Create(SaveNoteExecute, CanExecuteSave);
            DeleteNote = ReactiveCommand.Create(DeleteNoteExecute, CanExecuteDelete);
            SaveNote.Subscribe();
            DeleteNote.Subscribe();
            if (!String.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                _note = _noteService.OpenNote(path);
                CanDelete = true;
                _notePath = path;
            }
            else
            {
                _note = null;
                CanDelete = false;
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
                this.RaiseAndSetIfChanged(ref _noteTitle, value);
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
                this.RaiseAndSetIfChanged(ref _noteText, value);
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
                this.RaiseAndSetIfChanged(ref _createdString, value);
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
                this.RaiseAndSetIfChanged(ref _lastEditedString, value);
            }
        }

        private bool CanDelete
        {
            get
            {
                return _canDelete;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _canDelete, value);
            }
        }

        public ReactiveCommand<Unit, Unit> SaveNote { get; }
        public ReactiveCommand<Unit, Unit> DeleteNote { get; }
        public IObservable<bool> CanExecuteSave => this.WhenAnyValue(x => x.NoteTitleEntry, (NoteTitleEntry) => !string.IsNullOrEmpty(NoteTitleEntry));
        public IObservable<bool> CanExecuteDelete => this.WhenAnyValue(x => x.CanDelete);

        #endregion

        public void SaveNoteExecute()
        {
            if (_note != null)
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
            CanDelete = true;
            UpdateNoteView();
        }

        public void DeleteNoteExecute()
        {
            _noteService.DeleteNoteFile(_notePath);
            UpdateNoteView();
            CanDelete = false;
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