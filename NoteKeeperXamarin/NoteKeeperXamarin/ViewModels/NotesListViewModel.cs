using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperXamarin.ViewModels
{
    public class NotesListViewModel: INotifyPropertyChanged
    {
        private string[] _fileNames;
        private readonly NoteService _noteService;
        public event PropertyChangedEventHandler PropertyChanged;

        public NotesListViewModel(IStorageService service)
        {
            _noteService = new NoteService(service);
            _noteService.NotesChanged += UpdateNotesList;
            AddNote = new Command(async () => await AddNoteExecuteAsync());
            OpenNote = new Command<int>(async (id) => await OpenNoteExecuteAsync(id));
            DeleteNote = new Command<int>((id) => DeleteNoteExecute(id));
            _fileNames = _noteService.GetNamesOfAllExistingNoteFiles();
            CreateNotesList();
        }

        public Command AddNote { get; private set; }
        public Command OpenNote { get; private set; }
        public Command DeleteNote { get; private set; }
        public List<NoteItemModel> NoteItemList { get; private set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DeleteNoteExecute(int id)
        {
            _noteService.DeleteNoteFile(_fileNames[id]);
            OnPropertyChanged(nameof(NoteItemList));
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
            OnPropertyChanged(nameof(NoteItemList));
        }
    }
}
