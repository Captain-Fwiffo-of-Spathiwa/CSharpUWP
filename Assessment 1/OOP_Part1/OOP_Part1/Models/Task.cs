using System;



namespace OOP_Part1.Models
{
    internal class Task
    {
        private const string        DefaultDescription = "Default Task Description";

        // We want some validating on this. The assignment appears to be
        // only wanting a field with getter/setter functions for this one.
        private string              Description;

        public string               Notes = "";
        public bool                 IsComplete = false;
        public readonly DateTime    DateCreated;
        

        public Task(string description)
        {
            SetDescription(description);
            DateCreated = DateTime.Now;
        }

        public string GetDescription()
        {
            return Description;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                System.Diagnostics.Debug.WriteLine("Error - Blank Task description given.");
                Description = DefaultDescription;
                return;
            }

            Description = description;
        }

        public void ToggleCompletion()
        {
            IsComplete = !IsComplete;
        }
    }
}
