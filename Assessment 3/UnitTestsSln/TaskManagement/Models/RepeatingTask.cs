using TaskManagement.Helpers;
using System;
using System.IO;



namespace TaskManagement.Models
{
    /*
     *  Blurb:
     *  
     *  These tasks repeat every day or every week.
     *  When a task is “completed” it moves to the next date it needs to
     *  repeat. So, if you complete a weekly task on the 1st of March, it will
     *  shift seven days to the next week (the 8th of March).
     *  How you implement this is up to you.
    */

    public enum Frequency
    {
        Daily,
        Weekly
    }

    public class RepeatingTask : Task
    {
        public Frequency        Frequency;
        protected DateTime?     DateLastCompleted = null;

        /*
        * In C#, a readonly can only be set in the ctor of the class that
        * declared it. So we have to explicity call Task's ctor with the
        * description argument.
        */ 
        public RepeatingTask(string description, DateTime dueDate, Frequency frequency) : base(description)
        {
            DueDate = dueDate;  // A due date is mandatory for a RepeatingTask
            Frequency = frequency;
        }

        // A task is complete if the time it was last completed is inside the current due date cycle.
        // Eg., if the weekly task is due in 2 days, and the last completion time was 3 days ago,
        // the task is currently complete.
        public override bool IsComplete
        {
            get
            {
                if (DateLastCompleted == null)
                {
                    return false;
                }
                
                DateTime previousDueDate = (DateTime)DueDate - TimeSpan.FromDays(Frequency == Frequency.Daily? 1 : 7);
                return DateLastCompleted > previousDueDate;
            }
        }

        public override void ToggleCompletion()
        {
            if (IsComplete)
            {
                DateLastCompleted = null;
                return;
            }

            DateLastCompleted = DateTime.Now;
            RolloverDueDate();
        }

        private void RolloverDueDate()
        {
            while (DueDate < DateTime.Now)
            {
                DueDate = DueDate + TimeSpan.FromDays(Frequency == Frequency.Daily? 1 : 7);
            }
        }

        // Assessment 4 addition
        public override void SaveTo(BinaryWriter writer)
        {
            SaveUtils.SaveAndPrintInt(writer, 1);    // Indicator for which type of Task this is. 1 = "RepeatingTask"
            SaveDataTo(writer);
        }

        protected new void SaveDataTo(BinaryWriter writer)
        {
            base.SaveDataTo(writer);

            SaveUtils.SaveAndPrintInt(writer, (int)Frequency);
            SaveUtils.SaveAndPrintLong(writer, DateLastCompleted == null ? 0 : DateLastCompleted.Value.Ticks);
        }

        /// <summary>
        /// Destructively attempted a binary load.
        /// </summary>
        public override void LoadFrom(BinaryReader reader)
        {
            base.LoadFrom(reader);

            Frequency = (Frequency)SaveUtils.LoadAndPrintInt(reader);
            
            long dateLastCompletedTicks = SaveUtils.LoadAndPrintLong(reader);
            DateLastCompleted = dateLastCompletedTicks == 0 ? null : new(dateLastCompletedTicks);
        }

        public override string ToString()
        {
            return $"Repeating Task: {Description}\n";
        }
    }
}
