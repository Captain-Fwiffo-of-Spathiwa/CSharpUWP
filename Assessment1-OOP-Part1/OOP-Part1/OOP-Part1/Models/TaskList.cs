using System.Collections.Generic;

namespace OOP_Part1.Models
{
internal class TaskList
{
    private string      Name;
    private List<Task>  Tasks = new();

    // Lambda operator saying "Just return whatever this evaluates to". No setter desired.
    public int TotalTasksCount => Tasks.Count;
    public int IncompleteTasksCount
    {
        get
        {
            int incomplete = 0;
            foreach (var task in Tasks)
            {
                if (task.IsComplete)
                {
                    ++incomplete;
                }
            }

            return incomplete;
        }
    }


    TaskList(string name)
    {
        Name = name;
    }

    public void AddTask(Task task)
    {
        Tasks.Add(task);    // Duplicates welcome
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
