using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteKeeperXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteKeeperView : ContentPage
    {
        public NoteKeeperView(NoteService noteService, string path = null)
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