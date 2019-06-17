using NoteKeeperXamarin.Models;
using Splat;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class NoteService
    {
        private readonly IStorageService _storageService;
        private readonly string _noteFilesDirectory;

        public event EventHandler NotesChanged;

        public NoteService()
        {
            _storageService = Locator.Current.GetService<IStorageService>();
            _noteFilesDirectory = Path.Combine(FileSystem.AppDataDirectory, "SerializedNotes");
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        public NoteService(string path)
        {
            _storageService = Locator.Current.GetService<IStorageService>();
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

        public string SaveWithDynamicFileName(Note note)
        {
            string path = GetFullPathOfDirectoryAndFileName(note);
            _storageService.SaveToFile<Note>(note, path);
            OnNotesChanged(this, EventArgs.Empty);
            return path;
        }

        public void SaveWithDynamicFileName(Note note, string path)
        {
            _storageService.SaveToFile<Note>(note, path);
            OnNotesChanged(this, EventArgs.Empty);
        }

        public Note OpenNote(string fullPathName)
        {
            if (!File.Exists(fullPathName))
            {
                throw new FileNotFoundException("This note file doesn't exist!");
            }
            return _storageService.OpenFile<Note>(fullPathName);
        }

        public void DeleteNoteFile(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            _storageService.DeleteFile<Note>(path);
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