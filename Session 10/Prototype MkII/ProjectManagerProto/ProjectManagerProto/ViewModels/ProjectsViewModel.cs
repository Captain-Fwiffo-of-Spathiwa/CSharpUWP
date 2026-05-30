using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjectManagerProto.Models;
using Microsoft.Maui.Controls;

namespace ProjectManagerProto.ViewModels
{
    public class ProjectsViewModel
    {
        public ObservableCollection<Project> Projects { get; }
        public ICommand ProjectDoubleTappedCommand { get; }

#if WINDOWS
        private static readonly Dictionary<Project, Microsoft.Maui.Controls.Window> _openWindows = new();
#endif

        public ProjectsViewModel()
        {
            var sample = SampleData.Create();
            Projects = new ObservableCollection<Project>();
            foreach (var list in sample.GetTaskLists())
            {
                if (list is Project project)
                    Projects.Add(project);
            }

            ProjectDoubleTappedCommand = new Command<Project>(OnProjectDoubleClicked);
        }

        private async void OnProjectDoubleClicked(Project project)
        {
            // Dummy window for Windows only
#if WINDOWS
            Microsoft.UI.Xaml.Window mauiWinUIWindow;

            if (_openWindows.TryGetValue(project, out var existingWindow))
            {
                mauiWinUIWindow = existingWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (mauiWinUIWindow != null)
                {
                    await System.Threading.Tasks.Task.Delay(250);
                    mauiWinUIWindow.Activate();
                }
                return;
            }

            var window = new Microsoft.Maui.Controls.Window
            {
                Page = new ContentPage
                {
                    Content = new Label
                    {
                        Text = $"Project: {project?.Name ?? "Unknown"}",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }
                },
                Title = "Project Details"
            };

            _openWindows[project] = window;

            window.Destroying += (s, e) =>
            {
                _openWindows.Remove(project);
            };

            Application.Current.OpenWindow(window);
            
            mauiWinUIWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (mauiWinUIWindow != null)
            {
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
                    Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                        WinRT.Interop.WindowNative.GetWindowHandle(mauiWinUIWindow)
                    )
                );
                if (appWindow != null)
                {
                    appWindow.MoveAndResize(new Windows.Graphics.RectInt32(100, 700, 800, 600));
                }
                
                await System.Threading.Tasks.Task.Delay(250);
                mauiWinUIWindow?.Activate();
            }
#endif
        }

        public static void CloseAllProjectWindows()
        {
#if WINDOWS
            foreach (var win in _openWindows.Values)
            {
                //win.Close();
                Microsoft.Maui.Controls.Application.Current.CloseWindow(win);

            }
            _openWindows.Clear();
#endif
        }


    }
}