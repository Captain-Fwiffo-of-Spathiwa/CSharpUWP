using System.Collections.Generic;
using System.Linq;

namespace ProjectManagerProto.Models
{
    public class TaskList
    {
        private List<Task> _tasks = new List<Task>();
        private string _name;

        public int TotalTasksCount => _tasks.Count;
        public int IncompleteTasksCount => _tasks.Count(t => !t.IsComplete);

        public TaskList(string name) => _name = name;

        public string Name => _name;
        public string GetName() => _name;
        public void SetName(string name) => _name = name;
        public void AddTask(Task task) => _tasks.Add(task);
        public void DeleteAllCompletedTasks() => _tasks.RemoveAll(t => t.IsComplete);
        public IReadOnlyList<Task> GetTasks() => _tasks.AsReadOnly();
        public override string ToString() => _name;
        public DateTime DateCreated { get; set; }

    }
}