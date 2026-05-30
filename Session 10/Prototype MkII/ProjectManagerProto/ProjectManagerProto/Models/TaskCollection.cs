using System.Collections.Generic;
using System.Linq;

namespace ProjectManagerProto.Models
{
    public class TaskCollection
    {
        private List<TaskList> _taskLists = new List<TaskList>();

        public int TotalTasksCount => _taskLists.Sum(l => l.TotalTasksCount);
        public int IncompleteTasksCount => _taskLists.Sum(l => l.IncompleteTasksCount);

        public void AddTaskList(TaskList list) => _taskLists.Add(list);
        public List<TaskList> GetTaskLists() => _taskLists;
        public void DeleteAllCompletedTaskLists() => _taskLists.RemoveAll(l => l.IncompleteTasksCount == 0);
        public override string ToString() => $"TaskCollection: {_taskLists.Count} lists";
    }
}