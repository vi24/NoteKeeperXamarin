using NoteKeeperXamarin.EventArguments;
using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using ReactiveUI;
using Splat;
using System;
using System.Globalization;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NoteViewModel : ReactiveObject
    {
        private readonly INoteService _noteService;
        private Note _note;
        private string _notePath;
        private string _noteTitle;
        private string _noteText;
        private string _createdString;
        private string _lastEditedString;
        private bool _canDelete;
        private event NoteChangedEventHandler NoteChanged;
        private delegate void NoteChangedEventHandler(object sender, NoteChangedEventArgs args);

        public NoteViewModel(INoteService noteService = null, string noteID = null)
        {
            _noteService = noteService ?? Locator.Current.GetService<INoteService>();
            SaveNote = ReactiveCommand.CreateFromTask(SaveNoteExecute, CanExecuteSave);
            DeleteNote = ReactiveCommand.CreateFromTask(DeleteNoteExecute, CanExecuteDelete);
            NoteChanged += OpenNoteAsync;
            if (noteID != null)
            {
                OnNoteChanged(noteID);
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
        public IObservable<bool> CanExecuteSave => this.WhenAnyValue(x => x.NoteTitleEntry, (NoteTitleEntry) => (!string.IsNullOrEmpty(NoteTitleEntry) && new Regex("^[a-zA-Z0-9äöüÄÖÜ ]*$").IsMatch(NoteTitleEntry)));
        public IObservable<bool> CanExecuteDelete => this.WhenAnyValue(x => x.CanDelete);
        #endregion

        public async Task SaveNoteExecute()
        {
            if (_note != null)
            {
                _note.Title = NoteTitleEntry;
                _note.Text = NoteTextEditor;
                _note.LastEditedRoundTrip = DateTime.UtcNow.ToString("o");
                await _noteService.SaveNote(_note, _notePath);
            }
            else
            {
                DateTime created = DateTime.UtcNow;
                _note = new Note(NoteTitleEntry, NoteTextEditor, created.ToString("o"), created.ToString("o"));
                _notePath = await _noteService.SaveNote(_note);
            }
            CanDelete = true;
            UpdateNoteView();
        }

        public async Task DeleteNoteExecute()
        {
            await _noteService.DeleteNote(_notePath);
            UpdateNoteView();
            CanDelete = false;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void UpdateNoteView()
        {
            if (_note != null)
            {
                NoteTitleEntry = _note.Title;
                NoteTextEditor = _note.Text;
                DateTime created = DateTime.Parse(_note.CreatedRoundTrip);
                CreatedString = created.ToLocalTime().ToString("G", new CultureInfo("de-DE"));
                DateTime lastEdited = DateTime.Parse(_note.LastEditedRoundTrip);
                LastEditedString = lastEdited.ToLocalTime().ToString("G", new CultureInfo("de-DE"));
            }
            else
            {
                NoteTitleEntry = String.Empty;
                NoteTextEditor = String.Empty;
                CreatedString = String.Empty;
                LastEditedString = String.Empty;
            }
        }

        private void OnNoteChanged(string noteID)
        {
            NoteChanged?.Invoke(this, new NoteChangedEventArgs(noteID));
        }

        private async void OpenNoteAsync(object sender, NoteChangedEventArgs e)
        {
            try
            {
                _note = await _noteService.OpenNote(e.Path);
                CanDelete = true;
                _notePath = e.Path;
            }
            catch (Exception)
            {
                //Silently ignoring
            }
            UpdateNoteView();
        }
    }
}