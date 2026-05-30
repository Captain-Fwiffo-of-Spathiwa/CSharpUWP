using System.Linq;

namespace ProjectManagementProto.Models
{
    public class Project : TaskList
    {
        public float PercentComplete => TotalTasksCount == 0 ? 0 : (float)tasks.Count(t => t.IsComplete) / TotalTasksCount * 100;

        public Project(string name) : base(name) { }
    }
}