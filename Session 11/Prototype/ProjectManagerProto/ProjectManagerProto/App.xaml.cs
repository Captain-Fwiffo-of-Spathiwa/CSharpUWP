namespace ProjectManagerProto
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Title = "Project Manager Proto",
                Width = 1024,
                Height = 768
            };
        }
    }
}