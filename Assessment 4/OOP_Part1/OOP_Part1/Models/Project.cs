using System.Diagnostics;



namespace OOP_Part1.Models
{
    /*
    *  Blurb:
    *  
    *  While the project class is used differently to a normal task list, it
    *  is almost identical. There are three differences.
    *  • A calculated property that returns the percentage of the project
    *  that is complete.
    *  • Habits are not allowed in projects.
    *  • Repeating tasks are not allowed in projects.
    */

    internal class Project : TaskList
    {
        public float PercentComplete => (100.0f * (1.0f - (float)IncompleteTasksCount / (float)TotalTasksCount));

        public Project (string name) : base (name)
        {}

        public override void AddTask(Task task)
        {
            if (task is Habit || task is RepeatingTask)
            {
                Debug.WriteLine("Error - Projects can not contain RepeatingTasks or Habits!");
                return;
            }

            base.AddTask(task);
            Debug.WriteLine($"Added {task.GetDescription()} task to project.");
        }
    }
}
