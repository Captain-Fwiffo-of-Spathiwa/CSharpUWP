using Microsoft.VisualBasic;
using System;
using System.IO;
using TaskManagement.Helpers;



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

        public RepeatingTask(string description, DateTime dueDate, Frequency frequency) : base(description)
        {
            // Since Description is a readonly, we needed to call the base
            // class ctor to have it initialised.

            DueDate = dueDate;  // A due date is mandatory for a RepeatingTask
            Frequency = frequency;
        }

        public override bool IsComplete
        {
            /* ------------------------------------------------------------
             *  A RepeatingTask naturally changes from complete to
             *  incomplete as time passes, and is only reported as
             *  complete while today is still inside the due date window
             *  it was in when it was completed. Therefore it's not just a
             *  matter of which task is asked, but also when it is asked.
             *  
             *  Egs.: 
             *   - A weekly task that was completed on the 10th, and is
             *     due on the 11th, 18th, 25th, ... will report as
             *     complete if asked on the 10th or 11th, but incomplete
             *     if asked on the 12th.
             *  -  A daily task completed today will report as complete
             *     until tomorrow.
             * -----------------------------------------------------------*/
            get
            {
                if (DateLastCompleted == null)
                {
                    return false;
                }

                // Dailies only remain complete on the day of completion
                if (Frequency == Frequency.Daily)
                {
                    return (DateLastCompleted.Value - DateTime.Today).TotalDays >= 0;
                }

                // For weekly tasks, find the due date window that wraps today
                int daysUntilDue = (int)(DueDate.Value - DateTime.Today).TotalDays;
                int daysInWeek = 7;
                DateTime datePresentWindowWouldEnd = DateTime.Today + TimeSpan.FromDays(daysUntilDue % daysInWeek);

                // Was the last completion date inside that window?
                return (int)((datePresentWindowWouldEnd - DateLastCompleted.Value).TotalDays) < daysInWeek;
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

        /// <summary>
        /// Advance the DueDate to form a new due date window that starts
        /// in the future.
        /// </summary>
        private void RolloverDueDate()
        {
            /* ------------------------------------------------------------
             *  Advance the DueDate only if the task was due:
             *   - in the past
             *   - today (daily tasks)
             *   - this week (weekly tasks)
             *   
             *  Like all Tasks, a RepeatingTask could have a DueDate set
             *  far in the future. If a RepeatingTask's existing DueDate
             *  is already further from today than the size of its
             *  Frequency window (currently 1 week at most), then we don't
             *  advance that DueDate to be even further in the future.
             *  
             *  This means that a completed RepeatingTask can still be
             *  sensibly toggled back to incomplete and return to being
             *  completable by the DueDate it originally had.
             *  
             *  All other completed tasks, both due and overdue, will have
             *  their DueDates advanced to form the next available future
             *  due date window. For daily tasks, that is always tomorrow. 
             *  
             *  For weekly tasks, that is always "next week", where the
             *  exact day that defines when next week begins is calculated
             *  to always stay aligned with the existing DueDate. So:
             *   - completing a weekly that's due in 2 days will be
             *     advanced to be due in 9 days
             *   - completing an overdue weekly that was due 5 days ago
             *     will also be advanced to be due in 9 days (since "last
             *     week" ended 5 days ago, therefore "this week" ends in
             *     in 2 days, therfore "next week" ends in 9 days)
             * -----------------------------------------------------------*/ 

            int numDaysDueInThePast = (int)(DateTime.Now.Date - DueDate.Value.Date).TotalDays;
            
            if (Frequency == Frequency.Daily)
            {
                // Daily task: Was it due on or before today?
                if (numDaysDueInThePast >= 0)
                {
                    // Daily task DueDates are always advanced to "tomorrow"
                    DueDate = DateTime.Now + TimeSpan.FromDays(1);
                }
            }
            else
            {
                // Weekly task: Was it due in or before this week?
                int numDaysInWeek = 7;
                if (numDaysDueInThePast >= -numDaysInWeek + 1)
                {
                    // Weekly task DueDates are advanced to the next date
                    // that's in a future week. The border of what's a current
                    // or future week is controlled by the DueDate itself.
                    while ((DateTime.Now.Date - DueDate.Value.Date).TotalDays >= -numDaysInWeek + 1)
                    {
                        DueDate += TimeSpan.FromDays(numDaysInWeek);
                    }
                }
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
