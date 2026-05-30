namespace ProjectManagerProto.Models
{
    public struct Priority
    {
        public int Value { get; set; }
        public Priority(int value) => Value = value;
    }

    public class Task
    {
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime DateCreated { get; set; }
        public Priority TaskPriority { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsComplete { get; set; }
        public bool Overdue => DueDate.HasValue && !IsComplete && DueDate.Value < DateTime.Now;
        private string _description;

        public Task(string description)
        {
            Description = description;
            DateCreated = DateTime.Now;
            TaskPriority = new Priority(1);
        }

        //public string Description => _description;
        //public string GetDescription() => _description;
        //public void SetDescription(string desc) => _description = desc;
        public void ToggleCompletion() => IsComplete = !IsComplete;
        public override string ToString() => _description;
    }
}