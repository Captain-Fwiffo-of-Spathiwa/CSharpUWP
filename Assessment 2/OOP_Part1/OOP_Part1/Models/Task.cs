using System;



namespace OOP_Part1.Models
{
    struct Priority
    {
        public int Value = 0;

        public Priority(int value)
        {
            Value = value;
        }

        public static Priority operator ++(Priority toUpgrade)
        {
            return new Priority(Math.Min(toUpgrade.Value + 1, 1));
        }

        public static Priority operator --(Priority toDowngrade)
        {
            return new Priority(Math.Max(toDowngrade.Value - 1, -1));
        }
    }

    internal class Task
    {
        // -------- Assessment 1 fields ------------------------------------ //
        private const string        DefaultDescription  = "Default Task Desc";
        private string              Description;
        public string               Notes               = "";
        public readonly DateTime    DateCreated;


        // -------- Assessment 2 additions & changes ----------------------- //       
        public Priority             TaskPriority        = new();
        public DateTime?            DueDate             = null;

        // Changed from Assessment 1 to a virtual property due to its more
        // involved purpose in Assessment 2. External classes can't set it now.
        public virtual bool IsComplete
        {
            get;
            protected set;  // I'm never sure on YAGNI for private vs protected
        }

        public bool? Overdue
        {
            get
            {
                if (DueDate is null)
                {
                    return null;
                }

                return DueDate <= DateTime.Now;
            }
        }

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

        // Changed to virtual for Assessment 2 override
        public virtual void ToggleCompletion()
        {
            IsComplete = !IsComplete;
        }
    }
}
