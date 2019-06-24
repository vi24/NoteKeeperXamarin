using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.Views;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NotesListViewModel : ReactiveObject
    {
        private readonly INoteService _noteService;

        public event EventHandler ListUpdated;
        public NotesListViewModel()
        {
            _noteService = Locator.Current.GetService<INoteService>();
            _noteService.NotesChanged += UpdateNotesList;
            ListUpdated += UpdateNotesList;
            AddNoteCommand = ReactiveCommand.CreateFromTask<Unit>((Unit) => AddNoteExecuteAsync());
            OpenNoteCommand = ReactiveCommand.CreateFromTask<string>((filename) => OpenNoteExecuteAsync(filename));
            DeleteNoteCommand = ReactiveCommand.CreateFromTask<string>((filename) => DeleteNoteExecuteAsync(filename));
            OnListUpdated();
        }

        public ReactiveCommand<Unit, Unit> AddNoteCommand { get; }
        public ReactiveCommand<string, Unit> OpenNoteCommand { get; }
        public ReactiveCommand<string, Unit> DeleteNoteCommand { get; }
        public List<string> NoteItemList { get; private set; }

        private async Task DeleteNoteExecuteAsync(string name)
        {
            var file = NoteItemList.FindAll(n => n == name).SingleOrDefault();

            if (file.Count() == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Note not found", "The note that you have selected doesn't exist as a file", "Ok");
            }
            else
            {
                await _noteService.DeleteNote(file);
                this.RaisePropertyChanged(nameof(NoteItemList));
            }
        }

        private async Task AddNoteExecuteAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService));
        }

        private async Task OpenNoteExecuteAsync(string name)
        {
            var file = NoteItemList.FindAll(n => n == name).SingleOrDefault();

            if (file == null)
            {
                await Application.Current.MainPage.DisplayAlert("Note not found", "The note that you have selected doesn't exist as a file", "Ok");
            }
            else
            {
                await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService, file));
            }
        }

        private async void UpdateNotesList(object sender, EventArgs e)
        {
            NoteItemList = await _noteService.GetAllExistingNotesIDs();
            this.RaisePropertyChanged(nameof(NoteItemList));
        }

        private void OnListUpdated()
        {
            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
