using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace NoteKeeperXamarin.Models
{
    [DataContract]
    public class Note
    {

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
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string CreatedRoundTrip { get; private set; }

        [DataMember]
        public string LastEditedRoundTrip { get; set; }
    }
}
