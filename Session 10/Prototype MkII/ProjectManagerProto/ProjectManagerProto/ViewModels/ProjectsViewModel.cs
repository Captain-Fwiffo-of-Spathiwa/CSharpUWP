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

        private void OnProjectDoubleClicked(Project project)
        {
            // Dummy window for Windows only
#if WINDOWS
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
            Application.Current.OpenWindow(window);
#endif
        }
    }
}