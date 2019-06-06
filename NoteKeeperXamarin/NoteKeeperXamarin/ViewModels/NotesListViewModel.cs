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
        private string[] FileNames;
        private readonly NoteService _noteService;
        public event PropertyChangedEventHandler PropertyChanged;

        public NotesListViewModel(IStorageService service)
        {
            _noteService = new NoteService(service);
            _noteService.NotesChanged += UpdateNotesList;
            AddNote = new Command(async () => await AddNoteExecuteAsync());
            FileNames = _noteService.GetAllExistingNoteFiles();
            CreateNotesList();
        }

        public Command AddNote { get; private set; }
        public List<NoteItemModel> NoteItemList { get; private set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateNotesList()
        {
            NoteItemList = new List<NoteItemModel>();
            for (int i = 0; i < FileNames.Length; i++)
            {
                NoteItemList.Add(new NoteItemModel { ID = i, NoteFileName = Path.GetFileName(FileNames[i]) });
            }   
        }

        private void UpdateNotesList(object sender, EventArgs e)
        {
            FileNames = _noteService.GetAllExistingNoteFiles();
            CreateNotesList();
            OnPropertyChanged(nameof(NoteItemList));
        }

        async Task AddNoteExecuteAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NoteKeeperView(_noteService));
        }

    }
}
