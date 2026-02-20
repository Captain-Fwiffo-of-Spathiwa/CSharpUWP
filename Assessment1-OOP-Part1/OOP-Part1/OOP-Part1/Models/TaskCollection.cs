using System.Collections.Generic;



namespace OOP_Part1.Models
{
internal class TaskCollection
{
    private List<TaskList> TaskLists = new();

    public int TotalTasksCount
    {
        get
        {
            int total = 0;
            foreach (var taskList in TaskLists)
            {
                total += taskList.TotalTasksCount;
            }

            return total;
        }
    }

    public int IncompleteTasksCount
    {
        get
        {
            int incomplete = 0;
            foreach (var taskList in TaskLists)
            {
                incomplete += taskList.IncompleteTasksCount;
            }

            return incomplete;
        }
    }

    public void AddTaskList(TaskList taskList)
    {
        TaskLists.Add(taskList);
    }

    public void ClearAllCompletedTasks()
    {
        foreach (var taskList in TaskLists)
        {
            taskList.ClearCompletedTasks();
        }
    }
}
}
