using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NoteKeeperXamarin.Models;
using Splat;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class NoteListService : INoteService
    {
        private readonly IStorageService _storageService;
        private List<Note> _notes;

        public event EventHandler NotesChanged;

        public NoteListService (IStorageService storageService = null)
        {
            _storageService = storageService ?? Locator.Current.GetService<IStorageService>();
        }

        public async Task DeleteNote(string name)
        {
            if (String.IsNullOrEmpty(name)) return;
            await _storageService.Delete<Note>(name);
            OnNotesChanged();
        }

        public async Task<Note> OpenNote(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            return await _storageService.Open<Note>(name);
        }

        public async Task<string> SaveNote(Note note, string name=null)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }
            await _storageService.Save<Note>(note, name);
            OnNotesChanged();
            if (name == null) return String.Empty;
            return name;
        }

        private void OnNotesChanged()
        {
            NotesChanged?.Invoke(this, EventArgs.Empty);
        }

        public Task<List<string>> GetAllExistingNotesIDs()
        {
            List<string> list = new List<string>();
            return Task.FromResult(list);
        }
    }
}
