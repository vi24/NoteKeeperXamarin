using System;
using System.Runtime.Serialization;

namespace NoteKeeperXamarin.Model
{
    [DataContract]
    public class Note
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public DateTime LastEdited { get; set; }

        public Note (string title, string text, DateTime created, DateTime lastEdited)
        {
            Title = title;
            Text = text;
            Created = created;
            LastEdited = lastEdited;
        }
    }
}
