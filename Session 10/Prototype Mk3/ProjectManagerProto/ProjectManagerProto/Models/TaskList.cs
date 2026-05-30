using System.Collections.Generic;
using System.Linq;

namespace ProjectManagementProto.Models
{
    public class TaskList
    {
        public string Name { get; set; }
        protected List<Task> tasks = new List<Task>();

        public int TotalTasksCount => tasks.Count;
        public int IncompleteTasksCount => tasks.Count(t => !t.IsComplete);

        public TaskList(string name) { Name = name; }

        public void AddTask(Task task) => tasks.Add(task);
        public void DeleteAllCompletedTasks() => tasks.RemoveAll(t => t.IsComplete);
        public IReadOnlyList<Task> GetTasks() => tasks.AsReadOnly();
    }
}