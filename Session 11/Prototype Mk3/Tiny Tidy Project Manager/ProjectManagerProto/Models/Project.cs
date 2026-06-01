using ProjectManagerProto.Helpers;
using System.Diagnostics;



namespace ProjectManagerProto.Models
{
    /// <summary>
    /// While the Project class is used differently to a normal TaskList, it
    /// is almost identical. A calculated property returns the percentage of
    /// the Project that is complete.
    /// </summary>
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

        public override void SaveTo(BinaryWriter writer)
        {
            // Projects don't actually have any new data to save (or load).
            // This call exists purely to give the saved data a way to
            // identify that this is a Project, not a TaskList.

            // Indicator for which type of TaskList this is. 1 = "Project"
            SaveUtils.SaveAndPrintInt(writer, 1);

            SaveDataTo(writer);
        }
    }
}
