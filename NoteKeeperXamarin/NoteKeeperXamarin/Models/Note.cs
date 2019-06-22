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
        public string CreatedRoundTrip { get; }

        [DataMember]
        public string LastEditedRoundTrip { get; set; }

        public Note (string title, string text, string created, string lastEdited)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title shouldn't be empty");
            }

            Title = title;
            Text = text;
            CreatedRoundTrip = created;
            LastEditedRoundTrip = lastEdited;
        }
    }
}
