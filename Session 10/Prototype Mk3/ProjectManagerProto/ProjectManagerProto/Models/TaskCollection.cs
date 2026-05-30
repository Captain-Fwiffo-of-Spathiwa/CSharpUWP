using System.Collections.Generic;
using System.Linq;

namespace ProjectManagementProto.Models
{
    public class TaskCollection
    {
        private List<TaskList> taskLists = new List<TaskList>();

        public int TotalTasksCount => taskLists.Sum(l => l.TotalTasksCount);
        public int IncompleteTasksCount => taskLists.Sum(l => l.IncompleteTasksCount);

        public void AddTaskList(TaskList list) => taskLists.Add(list);
        public List<TaskList> GetTaskLists() => taskLists;
        public void DeleteAllCompletedTaskLists() => taskLists.RemoveAll(l => l.IncompleteTasksCount == 0);
    }
}