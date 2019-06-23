using NoteKeeperXamarin.Services;
using NoteKeeperXamarin.Views;
using Splat;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteKeeperXamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Locator.CurrentMutable.Register(() => new CSVStorageService(), typeof(IStorageService));
            Locator.CurrentMutable.Register(() => new NoteService(), typeof(INoteService));
            MainPage = new NavigationPage(new NoteListKeeperView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
