using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NotesListViewModel: ViewModelBase
    {
        private string[] _fileNames;
        private readonly NoteService _noteService;

        public NotesListViewModel(IStorageService service)
        {
            _noteService = new NoteService(service);
            _noteService.NotesChanged += UpdateNotesList;
            AddNoteCommand = ReactiveCommand.CreateFromTask<Unit>( (Unit) => AddNoteExecuteAsync());
            OpenNoteCommand = ReactiveCommand.CreateFromTask<int>((id) => OpenNoteExecuteAsync(id));
            DeleteNoteCommand = ReactiveCommand.Create<int>((id) => DeleteNoteExecute(id));
            AddNoteCommand.Subscribe();
            OpenNoteCommand.Subscribe();
            DeleteNoteCommand.Subscribe();
            _fileNames = _noteService.GetNamesOfAllExistingNoteFiles();
            CreateNotesList();
        }

        public ReactiveCommand<Unit,Unit> AddNoteCommand { get; }
        public ReactiveCommand<int ,Unit> OpenNoteCommand { get; }
        public ReactiveCommand<int ,Unit> DeleteNoteCommand { get; }
        public List<NoteItemModel> NoteItemList { get; private set; }

        private void DeleteNoteExecute(int id)
        {
            _noteService.DeleteNoteFile(_fileNames[id]);
            this.RaisePropertyChanged(nameof(NoteItemList));
        }

        private async Task AddNoteExecuteAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService, String.Empty));
        }

        private async Task OpenNoteExecuteAsync(int id)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService, _fileNames[id]));
        }

        private void CreateNotesList()
        {
            NoteItemList = new List<NoteItemModel>();
            for (int i = 0; i < _fileNames.Length; i++)
            {
                NoteItemList.Add(new NoteItemModel { Id = i, NoteFileName = Path.GetFileName(_fileNames[i]) });
            }   
        }

        private void UpdateNotesList(object sender, EventArgs e)
        {
            _fileNames = _noteService.GetNamesOfAllExistingNoteFiles();
            CreateNotesList();
            this.RaisePropertyChanged(nameof(NoteItemList));
        }
    }
}
