using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.Maui.LifecycleEvents;
using Windows.Graphics;

namespace ProjectManagerProto
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
                    events.AddWindows(windows =>
                    {
                        windows.OnWindowCreated(window =>
                        {
                            const int x = 20;
                            const int y = 20;
                            const int width = 800;
                            const int height = 1000;

                            var mauiWinUIWindow = (Microsoft.UI.Xaml.Window)window;
                            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow);
                            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                            var appWindow = AppWindow.GetFromWindowId(windowId);

                            if (appWindow is not null)
                            {
                                appWindow.MoveAndResize(new RectInt32(x, y, width, height));
                            }
                        });
                    });
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}