using NoteKeeperXamarin.Models;
using Splat;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class NoteService: INoteService
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

        private void OnNotesChanged()
        {
            NotesChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task<string> SaveNote(Note note)
        {
            string path = GetFullPathOfDirectoryAndFileName(note);
            await _storageService.Save<Note>(note, path);
            OnNotesChanged();
            return path;
        }

        public async Task SaveNote(Note note, string path)
        {
            await _storageService.Save<Note>(note, path);
            OnNotesChanged();
        }
        /// <summary>
        /// Opens a <see cref="Note"/> for a given path
        /// </summary>
        /// <param name="fullPathName"></param>
        /// <returns>A note</returns>
        /// <returns><see cref="FileNotFoundException"/> if File not found</returns>
        public async Task<Note> OpenNote(string fullPathName)
        {
            if (String.IsNullOrEmpty(fullPathName))
            {
                throw new ArgumentNullException(nameof(fullPathName));
            }
            return await _storageService.Open<Note>(fullPathName);
        }

        public async Task DeleteNote(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            await _storageService.Delete<Note>(path);
            OnNotesChanged();
        }

        public string[] GetPathsOfAllExistingNoteFiles()
        {
            return Directory.GetFiles(_noteFilesDirectory);
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
    }
}