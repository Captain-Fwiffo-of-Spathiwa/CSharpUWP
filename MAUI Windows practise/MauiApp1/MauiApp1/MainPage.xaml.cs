namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnDoubleClickBoxTapped(object sender, EventArgs e)
        {
            #if WINDOWS
            var window = new Window(new ContentPage());
            Application.Current.OpenWindow(window);
            await Task.Delay(250);
        
            var mauiWinUIWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            mauiWinUIWindow?.Activate();

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            
            appWindow.MoveAndResize(new Windows.Graphics.RectInt32(100, 100, 800, 600));
            #endif
        }
    }
}
