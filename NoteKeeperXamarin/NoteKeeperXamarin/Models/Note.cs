using System;
using System.Runtime.Serialization;

namespace NoteKeeperXamarin.Models
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
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title shouldn't be empty");
            }

            Title = title;
            Text = text;
            Created = created;
            LastEdited = lastEdited;
        }
    }
}
