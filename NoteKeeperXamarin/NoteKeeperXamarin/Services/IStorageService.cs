using System;
using System.Threading.Tasks;

namespace NoteKeeperXamarin.Services
{
    public interface IStorageService
    {
        string FileExtensionName { get; }
        Task Save<T>(T obj, string path);
        Task<T> Open<T>(string path);
        Task Delete<T>(string path);
    }
}
