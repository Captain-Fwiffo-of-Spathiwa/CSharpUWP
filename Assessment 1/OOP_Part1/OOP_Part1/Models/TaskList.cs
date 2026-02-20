using System.Collections.Generic;



namespace OOP_Part1.Models
{
    internal class TaskList
    {
        private const string DefaultName = "Just another TaskList";

        private string      Name;
        private List<Task>  Tasks = new();

        // Lambda operator saying "Just return whatever this lne evaluates to". No setter desired.
        public int TotalTasksCount => Tasks.Count;
        public int IncompleteTasksCount
        {
            get
            {
                int incomplete = 0;
                foreach (var task in Tasks)
                {
                    if (!task.IsComplete)
                    {
                        ++incomplete;
                    }
                }

                return incomplete;
            }
        }


        public TaskList(string name)
        {
            SetName(name);
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                System.Diagnostics.Debug.WriteLine("Error - Blank TaskList name given.");
                Name = DefaultName;
                return;
            }

            Name = name;
        }

        public void AddTask(Task task)
        {
            Tasks.Add(task);    // Duplicate pointers welcome
        }

        public void ClearCompletedTasks()
        {
            // This is nice. "Remove each item where some predicate based on that item is true".
            // The alternative I would have used would be a reverse indexed for loop, since C#
            // doesn't let you modify a list in place with iterators.
            Tasks.RemoveAll(task => task.IsComplete);
        }
    }
}
