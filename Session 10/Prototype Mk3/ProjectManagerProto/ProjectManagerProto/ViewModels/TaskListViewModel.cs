using System.Collections.ObjectModel;
using ProjectManagerProto.Models;
using Task = ProjectManagerProto.Models.Task;

namespace ProjectManagerProto.ViewModels
{
    public class TaskListViewModel
    {
        public ObservableCollection<Task> Tasks { get; }

        public TaskListViewModel() { Tasks = new ObservableCollection<Task>(); }

        public TaskListViewModel(Project project)
        {
            Tasks = new ObservableCollection<Task>(project.GetTasks());
        }
    }
}