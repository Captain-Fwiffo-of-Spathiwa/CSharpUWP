using System;
using System.Collections.Generic;

namespace ProjectManagementProto.Models
{
    public class Task
    {
        public string Notes { get; set; }
        public DateTime DateCreated { get; set; }
        public Priority TaskPriority { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsComplete { get; set; }
        public bool Overdue => DueDate.HasValue && !IsComplete && DueDate.Value < DateTime.Now;
        public string Description { get; set; }

        public Task(string description)
        {
            Description = description;
            DateCreated = DateTime.Now;
            TaskPriority = new Priority(1);
        }

        public string GetDescription() => Description;
        public void SetDescription(string desc) => Description = desc;
        public void ToggleCompletion() => IsComplete = !IsComplete;

        public struct Priority
        {
            public int Value { get; set; }
            public Priority(int value) { Value = value; }
        }
    }
}