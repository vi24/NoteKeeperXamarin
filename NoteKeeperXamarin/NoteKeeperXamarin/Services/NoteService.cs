using NoteKeeperXamarin.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class NoteService : INoteService
    {
        private readonly IStorageService _storageService;
        private readonly string _noteFilesDirectory;

        public event EventHandler NotesChanged;

        public NoteService(IStorageService storageService = null)
        {
            _storageService = storageService ?? Locator.Current.GetService<IStorageService>();
            _noteFilesDirectory = Path.Combine(FileSystem.AppDataDirectory, "SerializedNotes");
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        public NoteService(string path, IStorageService storageService = null)
        {
            _storageService = storageService ?? Locator.Current.GetService<IStorageService>();
            if (String.IsNullOrWhiteSpace(path))
            {
                _noteFilesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SerializedNotes");
            }
            else
            {
                _noteFilesDirectory = path;
            }
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        public async Task<string> SaveNote(Note note, string filename = null)
        {
            string path;
            if (filename == null)
            {
                path = GetFullPathOfDirectoryAndFileName(note);
            }
            else
            {
                path = Path.Combine(_noteFilesDirectory, filename);
            }
            await _storageService.Save<Note>(note, path);
            OnNotesChanged();
            return path;
        }

        /// <summary>
        /// Opens a <see cref="Note"/> for a given path
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>A note</returns>
        /// <returns><see cref="FileNotFoundException"/> if File not found</returns>
        public async Task<Note> OpenNote(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            string path = Path.Combine(_noteFilesDirectory, filename);
            return await _storageService.Open<Note>(path);
        }

        public async Task DeleteNote(string filename)
        {
            if (String.IsNullOrEmpty(filename)) return;
            string path = Path.Combine(_noteFilesDirectory, filename);
            await _storageService.Delete<Note>(path);
            OnNotesChanged();
        }

        public async Task<List<string>> GetAllExistingNotesIDs()
        {
            string[] paths = await Task.Run(() => Directory.GetFiles(_noteFilesDirectory));
            List<string> noteItemList = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                noteItemList.Add(Path.GetFileName(paths[i]));
            }
            return noteItemList;
        }

        private string GenerateFileName(Note note)
        {
            if (note == null) return String.Empty;
            DateTime dateTime = DateTime.Parse(note.CreatedRoundTrip, null, DateTimeStyles.RoundtripKind);
            return Regex.Replace(note.Title, @"\s+", "") + dateTime.ToFileTime() + _storageService.FileExtensionName;
        }

        private string GetFullPathOfDirectoryAndFileName(Note note)
        {
            if (note == null) return String.Empty;
            return Path.Combine(_noteFilesDirectory, GenerateFileName(note));
        }

        private void OnNotesChanged()
        {
            NotesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}