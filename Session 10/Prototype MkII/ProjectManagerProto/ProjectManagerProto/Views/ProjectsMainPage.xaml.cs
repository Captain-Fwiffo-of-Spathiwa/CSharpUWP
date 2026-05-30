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
            Window.Destroying += OnMainWindowDestroying;
        }

        private void OnMainWindowDestroying(object? sender, EventArgs e)
        {
            ViewModels.ProjectsViewModel.CloseAllProjectWindows();
        }

        void OnAddButtonPressed(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.Green;
        }

        void OnAddButtonReleased(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.LimeGreen;
        }

        void OnDeleteButtonPressed(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.DarkRed;
        }

        void OnDeleteButtonReleased(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Colors.Firebrick;
        }
    }
}