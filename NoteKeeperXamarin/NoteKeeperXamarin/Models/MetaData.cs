using System.Runtime.Serialization;

namespace NoteKeeperXamarin.Model
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
