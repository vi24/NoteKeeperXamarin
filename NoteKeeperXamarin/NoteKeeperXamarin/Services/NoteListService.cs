using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NoteKeeperXamarin.Models;
using Splat;

namespace NoteKeeperXamarin.Services
{
    public class NoteListService : INoteService
    {
        private readonly IStorageService _storageService;

        public event EventHandler NotesChanged;

        public NoteListService(IStorageService storageService = null)
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

        public async Task<string> SaveNote(Note note, string name = null)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }
            if (name == null)
            {
                name = GenerateIDName(note);
            }
            await _storageService.Save<Note>(note, name);
            OnNotesChanged();
            return name;
        }


        public async Task<List<string>> GetAllExistingNotesIDs()
        {
            if (_storageService is SingleCSVStorageService)
            {
                SingleCSVStorageService storageService = (SingleCSVStorageService)_storageService;
                string[] records = await storageService.GetAllRecordsIDs<Note>();
                return records.ToList();
            }
            else
            {
                List<string> list = new List<string>();
                return list;
            }
        }

        private void OnNotesChanged()
        {
            NotesChanged?.Invoke(this, EventArgs.Empty);
        }

        private string GenerateIDName(Note note)
        {
            if (note == null) return String.Empty;
            DateTime dateTime = DateTime.Parse(note.CreatedRoundTrip, null, DateTimeStyles.RoundtripKind);
            return Regex.Replace(note.Title, @"\s+", "") + dateTime.ToFileTime();
        }
    }
}
