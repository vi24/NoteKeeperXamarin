using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteKeeperXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteKeeperView : ContentPage
    {
        public NoteKeeperView(INoteService noteService = null, string path = null)
        {
            InitializeComponent();
            if (path == null)
            {
                this.BindingContext = new NoteViewModel(noteService);
            }
            else
            {
                this.BindingContext = new NoteViewModel(noteService, path);
            }
        }
    }
}