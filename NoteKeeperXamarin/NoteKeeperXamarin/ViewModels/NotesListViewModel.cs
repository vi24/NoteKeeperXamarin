using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NotesListViewModel: ReactiveObject
    {
        private string[] _filePaths;
        private readonly NoteService _noteService;

        public NotesListViewModel(IStorageService service)
        {
            _noteService = new NoteService(service);
            _noteService.NotesChanged += UpdateNotesList;
            AddNoteCommand = ReactiveCommand.CreateFromTask<Unit>( (Unit) => AddNoteExecuteAsync());
            OpenNoteCommand = ReactiveCommand.CreateFromTask<string>((filename) => OpenNoteExecuteAsync(filename));
            DeleteNoteCommand = ReactiveCommand.Create<string>((filename) => DeleteNoteExecute(filename));
            AddNoteCommand.Subscribe();
            OpenNoteCommand.Subscribe();
            DeleteNoteCommand.Subscribe();
            _filePaths = _noteService.GetPathsOfAllExistingNoteFiles();
            CreateNotesList();
        }

        public ReactiveCommand<Unit,Unit> AddNoteCommand { get; }
        public ReactiveCommand<string ,Unit> OpenNoteCommand { get; }
        public ReactiveCommand<string ,Unit> DeleteNoteCommand { get; }
        public List<string> NoteItemList { get; private set; }

        private void DeleteNoteExecute(string filename)
        {
            var file = from f in _filePaths
                       where Path.GetFileName(f) == filename
                       select f;
            _noteService.DeleteNoteFile(file.FirstOrDefault());
            this.RaisePropertyChanged(nameof(NoteItemList));
        }

        private async Task AddNoteExecuteAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService, String.Empty));
        }

        private async Task OpenNoteExecuteAsync(string filename)
        {
            var file = from f in _filePaths
                       where Path.GetFileName(f) == filename
                       select f;
            await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService, file.FirstOrDefault()));
        }

        private void CreateNotesList()
        {
            NoteItemList = new List<string>();
            for (int i = 0; i < _filePaths.Length; i++)
            {
                NoteItemList.Add(Path.GetFileName(_filePaths[i]));
            }   
        }

        private void UpdateNotesList(object sender, EventArgs e)
        {
            _filePaths = _noteService.GetPathsOfAllExistingNoteFiles();
            CreateNotesList();
            this.RaisePropertyChanged(nameof(NoteItemList));
        }
    }
}
