using System;
using System.Threading.Tasks;

namespace NoteKeeperXamarin.Services
{
    public interface IStorageService
    {
        string FileExtensionName { get; }
        Task SaveToFile<T>(T obj, string path);
        Task<T> OpenFile <T> (string path);
        Task DeleteFile<T>(string path);
    }
}
