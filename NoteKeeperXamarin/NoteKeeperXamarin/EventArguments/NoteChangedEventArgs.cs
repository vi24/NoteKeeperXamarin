using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeperXamarin.EventArguments
{
    public class NoteChangedEventArgs : EventArgs
    {
        private readonly string _path;

        public NoteChangedEventArgs(string path)
        {
            this._path = path;
        }

        public string Path
        {
            get { return this._path; }
        }
    }
}
