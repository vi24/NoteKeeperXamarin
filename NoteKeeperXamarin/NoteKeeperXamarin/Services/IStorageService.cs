using System;

namespace NoteKeeperXamarin.Services
{
    public interface IStorageService
    {
        string FileExtensionName { get; }
        void SaveToFile<T>(T obj, string path);
        T OpenFile <T> (string path);
        void DeleteFile<T>(string path);
    }
}
