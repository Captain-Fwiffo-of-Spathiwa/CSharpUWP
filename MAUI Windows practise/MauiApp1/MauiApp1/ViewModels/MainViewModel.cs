using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp1.Models;

#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
#endif


namespace MauiApp1.ViewModels
{
    public class EditDialogResult
    {
        public string Description { get; set; }
        public string Notes { get; set; }
        public int Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }

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

        //private async void OnItemDoubleClicked(DataItem selectedItem)
        //{
        //    if (selectedItem == null) return;

        //    #if WINDOWS
        //    var window = new Window(new ContentPage 
        //    { 
        //        Title = selectedItem.Title,
        //        Content = new Label 
        //        { 
        //            Text = selectedItem.Description, 
        //            HorizontalOptions = LayoutOptions.Center, 
        //            VerticalOptions = LayoutOptions.Center 
        //        } 
        //    });

        //    Application.Current.OpenWindow(window);
        //    await Task.Delay(250);

        //    var mauiWinUIWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        //    mauiWinUIWindow?.Activate();

        //    var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow);
        //    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
        //    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        //    appWindow.MoveAndResize(new Windows.Graphics.RectInt32(840, 20, 800, 600));
        //    #endif
        //}
#if WINDOWS
        //private async void OnItemDoubleClicked(DataItem selectedItem)
        //{
        //    if (selectedItem == null) return;

        //    var mauiWindow =  Microsoft.Maui.Controls.Application.Current.Windows.FirstOrDefault();
        //    var nativeWindow = mauiWindow?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        //    if (nativeWindow != null)
        //    {
        //        var dialog = new ContentDialog
        //        {
        //            Title = selectedItem.Title,
        //            Content = selectedItem.Description,
        //            CloseButtonText = "OK",
        //            XamlRoot = nativeWindow.Content.XamlRoot
        //        };
        //        await dialog.ShowAsync();
        //    }
        //}


        private async void OnItemDoubleClicked(DataItem selectedItem)
        {
            ShowEditDialogAsync("hey", "Ho", 2, DateTime.Now, DateTime.Now);
        }

        //private async void ShowEditDialog()
        //{
        //    var windows = Microsoft.Maui.Controls.Application.Current.Windows;
        //    var mauiWindow = windows.Count > 0 ? windows[0] : null;
        //    if (mauiWindow == null) return;

        //    var nativeWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        //    if (nativeWindow == null) return;

        //    // Create editable fields
        //    var stackPanel = new StackPanel();
        //    var textBox = new TextBox { PlaceholderText = "Enter something..." };
        //    stackPanel.Children.Add(textBox);

        //    var dialog = new ContentDialog
        //    {
        //        Title = "Edit Data",
        //        Content = stackPanel,
        //        PrimaryButtonText = "OK",
        //        CloseButtonText = "Cancel",
        //        XamlRoot = nativeWindow.Content.XamlRoot
        //    };

        //    var result = await dialog.ShowAsync();

        //    if (result == ContentDialogResult.Primary)
        //    {
        //        string userInput = textBox.Text;
        //        // Use userInput as needed
        //    }
        //}


        public async Task<EditDialogResult?> ShowEditDialogAsync(
            string description, string notes, int priority, DateTime dueDate, DateTime creationDate)
        {
            var windows = Microsoft.Maui.Controls.Application.Current.Windows;
            var mauiWindow = windows.Count > 0 ? windows[0] : null;
            if (mauiWindow == null) return null;

            var nativeWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (nativeWindow == null) return null;

            // Create controls
            var descriptionBox = new TextBox { Text = description, Header = "Description" };
            var notesBox = new TextBox { Text = notes, Header = "Notes", AcceptsReturn = true, Height = 60 };
            //var priorityBox = new NumberBox { Value = priority, Header = "Priority", Minimum = 0, Maximum = 100 };

            var priorityBox = new NumberBox { Value = priority, Minimum = 0, Maximum = 100000, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline };
            int newPriority = (int)priorityBox.Value;

            var dueDatePicker = new CalendarDatePicker { Header = "Due Date", Date = new DateTimeOffset(dueDate) };
            var creationDateBlock = new TextBlock { Text = creationDate.ToString("g") };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(descriptionBox);
            stackPanel.Children.Add(notesBox);
            stackPanel.Children.Add(priorityBox);
            stackPanel.Children.Add(dueDatePicker);
            stackPanel.Children.Add(new TextBlock { Text = "Creation Date", FontWeight = Microsoft.UI.Text.FontWeights.Bold });
            stackPanel.Children.Add(creationDateBlock);

            var dialog = new ContentDialog
            {
                Title = "Edit Item",
                Content = stackPanel,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                XamlRoot = nativeWindow.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                return new EditDialogResult
                {
                    Description = descriptionBox.Text,
                    Notes = notesBox.Text,
                    Priority = (int)priorityBox.Value,
                    DueDate = dueDatePicker.Date?.DateTime
                };
            }
            else
            {
                return null; // User cancelled
            }
        }
#endif


    }
}
