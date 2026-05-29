using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp1.Models;



namespace MauiApp1.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<DataItem> Items { get; set; }
        public ICommand ItemDoubleClickedCommand { get; set; }

        public MainViewModel()
        {
            Items = new ObservableCollection<DataItem>
            {
                new DataItem { Id = 1, Title = "First Item", Description = "Details for first item" },
                new DataItem { Id = 2, Title = "Second Item", Description = "Details for second item" },
                new DataItem { Id = 3, Title = "Third Item", Description = "Details for third item" }
            };

            ItemDoubleClickedCommand = new Command<DataItem>(OnItemDoubleClicked);
        }

        private async void OnItemDoubleClicked(DataItem selectedItem)
        {
            if (selectedItem == null) return;

#if WINDOWS
            var window = new Window(new ContentPage 
            { 
                Title = selectedItem.Title,
                Content = new Label 
                { 
                    Text = selectedItem.Description, 
                    HorizontalOptions = LayoutOptions.Center, 
                    VerticalOptions = LayoutOptions.Center 
                } 
            });
            
            Application.Current.OpenWindow(window);
            await Task.Delay(250);
        
            var mauiWinUIWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            mauiWinUIWindow?.Activate();

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            
            appWindow.MoveAndResize(new Windows.Graphics.RectInt32(200, 200, 1366, 768));
#endif
        }
    }
}
