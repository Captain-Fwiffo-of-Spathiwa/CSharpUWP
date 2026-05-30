using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using ProjectManagerProto.Views;
using System.Diagnostics;



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

        private void OnProjectDoubleClicked(object? project)
        {
#if WINDOWS
            var mauiWinUIWindow = new Microsoft.Maui.Controls.Window
            {
                Page = new ContentPage
                {
                    Content = new Label
                    {
                        Text = "Dummy Window",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 24
                    }
                },
                Title = "Dummy Project Window"
            };
            Application.Current?.OpenWindow(mauiWinUIWindow);
#endif
            Debug.WriteLine("Double-clicked project: " + project);
        }

    }
}