using System.Runtime.Serialization;

namespace NoteKeeperXamarin.Models
{
    [DataContract]
    public class MetaData
    {
        [DataMember]
        public string LastSavedNotePath { get; set; }

        public MetaData (string path)
        {
            LastSavedNotePath = path;
        }
    }
}
