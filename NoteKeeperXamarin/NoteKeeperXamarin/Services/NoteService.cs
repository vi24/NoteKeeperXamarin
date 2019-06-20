using NoteKeeperXamarin.Models;
using Splat;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class NoteService
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

        private void OnNotesChanged(object sender, EventArgs e)
        {
            NotesChanged?.Invoke(this, e);
        }

        public async Task<string> SaveWithDynamicFileName(Note note)
        {
            string path = GetFullPathOfDirectoryAndFileName(note);
            await _storageService.SaveToFile<Note>(note, path);
            OnNotesChanged(this, EventArgs.Empty);
            return path;
        }

        public async Task SaveWithDynamicFileName(Note note, string path)
        {
            await _storageService.SaveToFile<Note>(note, path);
            OnNotesChanged(this, EventArgs.Empty);
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
            return await _storageService.OpenFile<Note>(fullPathName);
        }

        public async Task DeleteNoteFile(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            await _storageService.DeleteFile<Note>(path);
            OnNotesChanged(this, EventArgs.Empty);
        }

        public string[] GetPathsOfAllExistingNoteFiles()
        {
            return Directory.GetFiles(_noteFilesDirectory);
        }

        private string GenerateFileName(Note note)
        {
            if (note == null) return String.Empty;
            return Regex.Replace(note.Title, @"\s+", "") + note.Created.ToFileTime() + _storageService.FileExtensionName;
        }

        private string GetFullPathOfDirectoryAndFileName(Note note)
        {
            if (note == null) return String.Empty;
            return Path.Combine(_noteFilesDirectory, GenerateFileName(note));
        }
    }
}