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
        private const string STATIC_FILE_NAME = "foo";
        private readonly IStorageService _storageService;
        private readonly string _noteFilesDirectory;

        public event EventHandler NotesChanged;

        public NoteService()
        {
            _storageService = Locator.Current.GetService<IStorageService>();
            _noteFilesDirectory = Path.Combine(FileSystem.AppDataDirectory, "SerializedNotes");
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        private void OnNotesChanged(object sender, EventArgs e)
        {
            NotesChanged?.Invoke(this, e);
        }

        public NoteService(IStorageService service, string path)
        {
            if (service == null)
            {
                _storageService = new JSONStorageService();
            }
            else
            {
                _storageService = service;
            }
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

        public void SaveWithStaticFileName(Note note)
        { 
            _storageService.SaveToFile<Note>(note, Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName));
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

        public Note OpenLastSavedNote()
        {
            string pathToLastSavedNote = Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName);
            if (!File.Exists(pathToLastSavedNote)) return new Note();
            Note note = _storageService.OpenFile<Note>(Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName));
            return note;
        }

        public void DeleteFooNoteFile()
        {
            string path = Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName);
            _storageService.DeleteFile<Note>(path);
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