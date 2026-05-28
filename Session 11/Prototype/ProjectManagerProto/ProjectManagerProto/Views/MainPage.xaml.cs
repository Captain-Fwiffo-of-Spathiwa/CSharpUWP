using ProjectManagerProto.ViewModels;
using TaskManagement.Models;
#if WINDOWS
using System.Runtime.InteropServices;
using WinRT.Interop;
#endif

namespace ProjectManagerProto.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel = new();
    private readonly Dictionary<Project, Window> _openProjectWindows = new();
    private bool _isDestroyingSubscribed;


    public MainPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;

        Loaded += OnLoaded;
    }

    /// <summary>
    /// We need to wait for the Loaded event so that Window isn't still
    /// null, so we can subscribe to its Destroying event in order to
    /// apply "One close closes them all".
    /// </summary>
    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_isDestroyingSubscribed || Window is null)
        {
            return;
        }

        _isDestroyingSubscribed = true;
        Window.Destroying += OnMainWindowDestroying;
    }

    private void OnMainWindowDestroying(object? sender, EventArgs e)
    {
        foreach (var window in _openProjectWindows.Values.ToList())
        {
            Application.Current?.CloseWindow(window);
        }

        _openProjectWindows.Clear();
    }

    private async void OnAddProjectClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Project", "Project name");

        if (!string.IsNullOrWhiteSpace(name))
        {
            _viewModel.AddProject(name);
        }
    }

    private void OnProjectDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject bindableObject)
        {
            return;
        }

        if (bindableObject.BindingContext is not MainViewModel.ProjectItemViewModel projectItem)
        {
            return;
        }

        if (_openProjectWindows.TryGetValue(projectItem.Project, out var existingWindow))
        {

            // We need this delay on bringing the Project's window to the front,
            // otherwise the app's main window reclaims focus while it's still
            // finishing with the DoubleTap gesture.
#if WINDOWS
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await System.Threading.Tasks.Task.Delay(250);
                BringWindowToFront(existingWindow);
            });
#endif

            return;
        }

        var page = new ProjectWindowPage(new ProjectViewModel(projectItem.Project));
        page.ProjectChanged += (_, _) => _viewModel.RefreshProjects();
        var window = new Window(page)
        {
            Title = projectItem.Name,
            Width = 800,
            Height = 450
        };
        
        Project project = projectItem.Project;
        window.Destroying += (_, _) => _openProjectWindows.Remove(project);

        _openProjectWindows[projectItem.Project] = window;

        Application.Current?.OpenWindow(window);
        
        #if WINDOWS
        BringWindowToFront(window);
        #endif
    }

#if WINDOWS
    private static async void BringWindowToFront(Window mauiWindow)
    {
        await System.Threading.Tasks.Task.Delay(50);

        if (mauiWindow.Handler?.PlatformView is not Microsoft.UI.Xaml.Window nativeWindow)
        {
            return;
        }

        nativeWindow.Activate();

        var hwnd = WindowNative.GetWindowHandle(nativeWindow);

        ShowWindow(hwnd, SW_RESTORE);
        BringWindowToTop(hwnd);
        SetForegroundWindow(hwnd);
    }

private const int SW_RESTORE = 9;

[DllImport("user32.dll")]
private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

[DllImport("user32.dll")]
private static extern bool BringWindowToTop(IntPtr hWnd);

[DllImport("user32.dll")]
private static extern bool SetForegroundWindow(IntPtr hWnd);
#endif
}