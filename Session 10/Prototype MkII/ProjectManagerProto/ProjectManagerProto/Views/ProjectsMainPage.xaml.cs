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
    }
}