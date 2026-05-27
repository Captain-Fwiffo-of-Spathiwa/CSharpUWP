using TaskManagement.Helpers;
using System;
using System.IO;



namespace TaskManagement.Models
{
    /// <summary>
    /// A simple priority value that can be increased and decreased with
    /// operators.
    /// </summary>
    public struct Priority
    {
        /// <summary>
        /// The priority's value. Lower value means higher priority.
        /// </summary>
        public int Value = 0;

        public Priority(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Increase a Priority's value
        /// </summary>
        /// <param name="toUpgrade">Priority to update.</param>
        /// <returns>The updated Priority.</returns>
        public static Priority operator ++(Priority toUpgrade)
        {
            return new Priority(Math.Min(toUpgrade.Value + 1, 1));
        }

        /// <summary>
        /// Decrease a Priority's value
        /// </summary>
        /// <param name="toDowngrade">Priority to update.</param>
        /// <returns>The updated Priority.</returns>
        public static Priority operator --(Priority toDowngrade)
        {
            return new Priority(Math.Max(toDowngrade.Value - 1, -1));
        }
    }

    /// <summary>
    /// Representation of a job that needs doing with an optional due date.
    /// Tasks can be toggled complete, prioritised and have notes attached.
    /// Where applicable, a Task's overdue status can be queried.
    /// </summary>
    public class Task
    {
        protected string            Description;

        /// <summary>
        ///  Extra notes for this Task. Optional.
        /// </summary>
        public string               Notes               = "";
        
        /// <summary>
        /// The date the Task was created.
        /// </summary>
        public DateTime             DateCreated;

        /// <summary>
        /// This Task's priority. Lower value means higher priority.
        /// A Task's priority value defaults to 0 (highest priority).
        /// </summary>
        public Priority             TaskPriority        = new();

        /// <summary>
        /// The date this Task is due. Optional.
        /// </summary>
        public DateTime?            DueDate             = null;

        /// <summary>
        /// Return whether the Task is complete.
        /// </summary>
        public virtual bool IsComplete
        {
            get;
            protected set;  // I'm never sure on YAGNI for private vs protected
        }

        /// <summary>
        /// Return whether the Task is overdue.
        /// </summary>
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

        /// <summary>
        /// Create a new Task.
        /// </summary>
        /// <param name="description">Name or description of the Task to be created.</param>
        /// <exception cref="ArgumentException"></exception>
        public Task(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Invalid description given for new Task.");
            }

            SetDescription(description);
            DateCreated = DateTime.Now;
        }

        /// <summary>
        /// Get a Task's description text.
        /// </summary>
        /// <returns>The Task's description.</returns>
        public string GetDescription()
        {
            return Description;
        }


        /// <summary>
        /// Set a Task's description text.
        /// </summary>
        /// <param name="description">New description to give the Task. Cannot be blank.</param>
        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                System.Diagnostics.Debug.WriteLine("Error - Blank Task description given.");
                return;
            }

            Description = description;
        }

        /// <summary>
        /// Toggle whether the Task is marked as complete.
        /// </summary>
        public virtual void ToggleCompletion()
        {
            IsComplete = !IsComplete;
        }

        /// <summary>
        /// Save the Task to a binary save file
        /// </summary>
        /// <param name="writer">BinaryWriter used to write to file.</param>
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
        /// Destructively attempted a binary load. This does not protect against bad saved
        /// data and will corrupt the Task if so. Do not use on Tasks that are not backed up.
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

        /// <summary>
        /// Get a string representation of the Task.
        /// </summary>
        /// <returns>A string representation of the Task.</returns>
        public override string ToString()
        {
            return $"Task: {Description}\n";
        }
    }
}
