﻿using NoteKeeperXamarin.Models;
using NoteKeeperXamarin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Essentials;

namespace NoteKeeperXamarin.Services
{
    public class NoteService
    {
        private const string STATIC_FILE_NAME = "foo";
        private const string METADATA_FILE_NAME = "metadata";
        private readonly IStorageService _storageService;
        private readonly string _noteFilesDirectory;
        private readonly string _metaDataDirectory = FileSystem.AppDataDirectory;

        public event EventHandler NotesChanged;

        public NoteService(IStorageService service)
        {
            if (service == null)
            {
                _storageService = new JSONStorageService();
            }
            else
            {
                _storageService = service;
            }
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
            OpenLastSavedNoteViaMetaData();
            Directory.CreateDirectory(_noteFilesDirectory);
        }

        public Note Note { get; set; }
        public MetaData MetaData { get; private set; }

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
            OnNotesChanged(this, EventArgs.Empty);
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

        public void DeleteFooNoteFile()
        {
            string path = Path.Combine(_noteFilesDirectory, STATIC_FILE_NAME + _storageService.FileExtensionName);
            _storageService.DeleteFile<Note>(path);
        }

        public void DeleteNoteFile()
        {
            string path = GetFullPathOfDirectoryAndFileName();
            if (String.IsNullOrEmpty(path)) return;
            _storageService.DeleteFile<Note>(path);
            OnNotesChanged(this, EventArgs.Empty);
        }

        public void DeleteNoteFile(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            _storageService.DeleteFile<Note>(path);
            OnNotesChanged(this, EventArgs.Empty);
        }

        public string[] GetNamesOfAllExistingNoteFiles()
        {
            return Directory.GetFiles(_noteFilesDirectory);
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