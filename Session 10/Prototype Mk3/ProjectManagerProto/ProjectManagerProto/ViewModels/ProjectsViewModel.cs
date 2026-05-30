using System.Collections.ObjectModel;
using ProjectManagerProto.Models;

namespace ProjectManagerProto.ViewModels
{
    public class ProjectsViewModel
    {
        public ObservableCollection<Project> Projects { get; }

        public ProjectsViewModel()
        {
            var sample = SampleData.Create();
            Projects = new ObservableCollection<Project>();
            foreach (var list in sample.GetTaskLists())
            {
                if (list is Project project)
                    Projects.Add(project);
            }
        }
    }
}