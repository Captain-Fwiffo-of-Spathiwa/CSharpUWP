using Microsoft.Maui.Controls;

namespace ProjectManagerProto.Views
{
    public partial class ProjectsMainPage : ContentPage
    {
        public ProjectsMainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("message\n\n\n\n\n\n\n");
            Window.Destroying += OnMainWindowDestroying;

        }

        private void OnMainWindowDestroying(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("\n\n\n\n\n\n\n\nDest");
        }
    }
}