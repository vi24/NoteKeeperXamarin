using NoteKeeperXamarin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoteKeeperXamarin.Services
{
    public interface INoteService
    {
        event EventHandler NotesChanged;
        Task<string> SaveNote(Note note, string name = null);
        Task<Note> OpenNote(string name);
        Task DeleteNote(string name);
    }
}
