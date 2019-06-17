using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.ViewModels;
using Splat;
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
    public partial class NoteListKeeperView : ContentPage
    {
        public NoteListKeeperView()
        {
            InitializeComponent();
            this.BindingContext = new NotesListViewModel();
        }
    }
}