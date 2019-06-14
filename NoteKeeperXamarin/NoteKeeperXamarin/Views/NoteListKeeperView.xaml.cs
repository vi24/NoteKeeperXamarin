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
            Locator.CurrentMutable.Register(() => new JSONStorageService(), typeof(IStorageService));
            this.BindingContext = new NotesListViewModel();
        }
    }
}