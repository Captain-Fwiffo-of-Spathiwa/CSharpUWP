using System.Linq;

namespace ProjectManagerProto.Models
{
    public class Project : TaskList
    {
        public DateTime DateCreated { get; set; }
        public float PercentComplete =>
            TotalTasksCount == 0 ? 0 : (float)GetTasks().Count(t => t.IsComplete) / TotalTasksCount * 100;

        public Project(string name) : base(name)
        {
            DateCreated = DateTime.Now;
        }
    }
}