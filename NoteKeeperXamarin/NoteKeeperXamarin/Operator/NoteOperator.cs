using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NoteKeeperXamarin.Operator
{
    public class NoteOperator
    {
        private const string STATIC_FILE_NAME = "foo";
        private const string METADATA_FILE_NAME = "metadata";
        private readonly IStorageService _storageService;
        private readonly string _noteFilesDirectory;
        private readonly string _metaDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public Note Note { get; set; }
        public MetaData MetaData { get; private set; }

        public NoteOperator(IStorageService service, string path)
        {
            OpenLastSavedNote();
            _storageService = service;
            _noteFilesDirectory = path;
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        public NoteOperator(IStorageService service)
        {
            _storageService = service;
            _noteFilesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SerializedNotes");
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        public void SaveWithStaticFileName(string title, string text)
        {
            if (Note != null)
            {
                Note.Title = title;
                Note.Text = text;
                Note.LastEdited = DateTime.Now;
            }
            else
            {
                Note = new Note(title, text, DateTime.Now, DateTime.Now);
            }
            _storageService.SaveToFile<Note>(Note, Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName));
        }

        public void SaveWithDynamicFileName(string title, string text)
        {
            if (Note != null && Note.Title == title)
            {
                Note.LastEdited = DateTime.Now;
                Note.Text = text;
            }
            else
            {
                Note = new Note(title, text, DateTime.Now, DateTime.Now);
            }
            _storageService.SaveToFile<Note>(Note, GetFullPathOfDirectoryAndFileName());
            WriteLastSavedNoteToMetaDataFile();
        }

        public void OpenNote(string fullPathName)
        {
            if (!File.Exists(fullPathName)) return;
            Note = _storageService.OpenFile<Note>(fullPathName);
        }
        public void OpenLastSavedNote()
        {
            string pathToLastSavedNote = Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName);
            if (!File.Exists(pathToLastSavedNote)) return;
            Note = _storageService.OpenFile<Note>(Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName));
        }

        public void OpenLastSavedNoteViaMetaData()
        {
            string pathToMetaDataFile = Path.Combine(_metaDataDirectory, METADATA_FILE_NAME + _storageService.FileExtensionName);
            if (!File.Exists(pathToMetaDataFile)) return;
            MetaData = _storageService.OpenFile<MetaData>(pathToMetaDataFile);
            OpenNote(Path.Combine(_noteFilesDirectory, MetaData.LastSavedNotePath));
        }

        public void DeleteNote()
        {
            string path = Path.Combine(_metaDataDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName);
            _storageService.DeleteFile<Note>(path);
        }

        private string GenerateFileName()
        {
            if (Note == null) return String.Empty;
            return Regex.Replace(Note.Title, @"\s+", "") + Note.Created.ToFileTime() + _storageService.FileExtensionName;
        }

        private string GetFullPathOfDirectoryAndFileName()
        {
            if (Note == null) return String.Empty;
            return Path.Combine(_noteFilesDirectory, GenerateFileName());
        }

        private void WriteLastSavedNoteToMetaDataFile()
        {
            if (Note == null) return;
            MetaData = new MetaData(GenerateFileName());
            _storageService.SaveToFile<MetaData>(MetaData, Path.Combine(_metaDataDirectory, METADATA_FILE_NAME + _storageService.FileExtensionName));
        }

        
    }
}
