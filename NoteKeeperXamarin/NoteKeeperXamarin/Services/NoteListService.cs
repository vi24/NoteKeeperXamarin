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
        private readonly string _noteDirectory;
        private List<Note> _notes;

        public event EventHandler NotesChanged;

        public NoteListService (IStorageService storageService = null)
        {
            _storageService = storageService ?? Locator.Current.GetService<IStorageService>();
            _noteDirectory = Path.Combine(FileSystem.AppDataDirectory, "SerializedNotes");
            Directory.CreateDirectory(_noteDirectory);
        }

        public NoteListService (string name, IStorageService storageService = null)
        {
            _storageService = storageService ?? Locator.Current.GetService<IStorageService>();
            if (String.IsNullOrWhiteSpace(name))
            {
                _noteDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SerializedNotes");
            }
            else
            {
                _noteDirectory = name;
            }
            Directory.CreateDirectory(_noteDirectory);
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

        public async Task SaveNote(Note note, string filename=null)
        {
            if (String.IsNullOrEmpty(filename))
            {
                filename = "notes";
            }
            string path = Path.Combine(_noteDirectory, filename + _storageService.FileExtensionName);
            await _storageService.Save<Note>(note, path);
            OnNotesChanged();
        }

        private void OnNotesChanged()
        {
            NotesChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}
