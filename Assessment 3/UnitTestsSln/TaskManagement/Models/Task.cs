using TaskManagement.Helpers;
using System;
using System.IO;



namespace TaskManagement.Models
{
    public struct Priority
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

    public class Task
    {
        // -------- Assessment 1 fields ------------------------------------ //
        private const string        DefaultDescription  = "Default Task Desc";
        protected string            Description;
        public string               Notes               = "";
        public DateTime             DateCreated;


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

        // Assessment 4 addition
        public virtual void SaveTo(BinaryWriter writer)
        {
            SaveUtils.SaveAndPrintInt(writer, 0);    // Indicator for which type of Task this is. 0 = "Base class"
            SaveDataTo(writer);
        }

        protected void SaveDataTo(BinaryWriter writer)
        {
            SaveUtils.SaveAndPrintString(writer, Description);
            SaveUtils.SaveAndPrintString(writer, Notes);
            SaveUtils.SaveAndPrintLong(writer, DateCreated.Ticks);
            SaveUtils.SaveAndPrintInt(writer, TaskPriority.Value);
            SaveUtils.SaveAndPrintLong(writer, DueDate == null ? 0 : DueDate.Value.Ticks);
            SaveUtils.SaveAndPrintBool(writer, IsComplete);
        }

        /// <summary>
        /// Destructively attempted a binary load.
        /// </summary>
        public virtual void LoadFrom(BinaryReader reader)
        {
            Description         = SaveUtils.LoadAndPrintString(reader);
            Notes               = SaveUtils.LoadAndPrintString(reader);
            DateCreated         = new (SaveUtils.LoadAndPrintLong(reader));
            TaskPriority.Value  = SaveUtils.LoadAndPrintInt(reader);

            long dueDateTicks   = SaveUtils.LoadAndPrintLong(reader);
            DueDate = dueDateTicks == 0 ? null : new(dueDateTicks);

            IsComplete          = SaveUtils.LoadAndPrintBool(reader);
        }

        public override string ToString()
        {
            return $"Task: {Description}\n";
        }
    }
}
