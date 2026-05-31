using System.Diagnostics;



namespace ProjectManagerProto.Models
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

    public class Project : TaskList
    {
        public float PercentComplete => TotalTasksCount == 0 ? 0 : (100.0f * (1.0f - (float)IncompleteTasksCount / (float)TotalTasksCount));

        public Project (string name) : base (name)
        {}

        public override void AddTask(Task task)
        {
            base.AddTask(task);
            Debug.WriteLine($"Added {task.GetDescription()} task to project.");
        }
    }
}
