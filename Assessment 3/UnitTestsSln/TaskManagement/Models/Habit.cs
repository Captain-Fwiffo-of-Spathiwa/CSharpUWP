using System;
using System.IO;
using TaskManagement.Helpers;



namespace TaskManagement.Models
{
    public class Habit : RepeatingTask
    {
        /*
        *  Blurb:
        *  
        *  A habit keeps track of how long you have successfully been
        *  completing the task. For example, if you have a habit task for
        *  exercise and you have exercised every day for five days, it would
        *  know you have a streak of five days. If you miss a day, the streak
        *  is broken and resets to zero.
        */

        public int              Streak { get; private set; } = 0;
        private DateTime?       LastDueDateSatisfied = null;

        public Habit(string description, DateTime dueDate, Frequency frequency, int dummyStreak)
            : base(description, dueDate, frequency)
        { 
            /* ------------------------------------------------------------
             *  ~~ NOTE - DUMMY VALUES ~~
             *  
             *  We are using dummy Streak, DateLastCompleted,
             *  and LastDueDateSatisfied values for demonstration purposes.
             *  
             *  The constructor sets Streak as the dummy value passed in,
             *  and we make use of the user's ability to set due dates in
             *  the past to set up dummy "last complete" and "last
             *  satisfied" dates.
             *  
             *  The result is that this instance is now pretending:
             *  "Yeah I had a Streak of this many, that was achieved
             *  in the past one cycle before the input due date."
             * 
             *  Any completed task now will react as if the task was
             *  indeed completed at that point in the past, which will
             *  or won't result in a continued streak.
             * 
             *  This constructor is otherwise expected to have no code.
             * -----------------------------------------------------------*/ 

            Streak = dummyStreak;

            if (dummyStreak > 0)
            {
                DateLastCompleted = dueDate - TimeSpan.FromDays(Frequency == Frequency.Daily ? 1 : 7);
                LastDueDateSatisfied = dueDate - TimeSpan.FromDays(Frequency == Frequency.Daily ? 1 : 7);
            }
        }

        public override void ToggleCompletion()
        {
            // Toggle the task's completion status via the parent
            base.ToggleCompletion();

            DateTime previousDueDate = GetEndOfPreviousDueDateWindow();

            // See if a completed task continues the streak
            if (IsComplete)
            {
                /* --------------------------------------------------------
                 *  If the Habit has just been completed, see if the last
                 *  due date that was satsified is the same as due date of
                 *  the window that was 1 window in the past.
                 *  If it is, the streak continues.
                 * -------------------------------------------------------*/

                if (LastDueDateSatisfied != null && LastDueDateSatisfied.Value.Date == previousDueDate)
                {
                    ++Streak;
                }
                else
                {
                    Streak = 1;
                }
                
                LastDueDateSatisfied = DueDate;
            }
            // Else, an un-completed task needs to unwind its streak
            else
            {
                --Streak;

                if (Streak > 0)
                {
                    LastDueDateSatisfied = previousDueDate;
                }
                else
                {
                    LastDueDateSatisfied = null;
                }
            }
        }

        private DateTime GetEndOfPreviousDueDateWindow()
        {
            if (Frequency == Frequency.Daily)
            {
                return DateTime.Today - TimeSpan.FromDays(1);
            }

            int daysInAWeek = 7;
            int daysUntilEndOfWindow;
            if (DueDate.Value.Date >= DateTime.Today)
            {
                daysUntilEndOfWindow = (int)(DueDate.Value.Date - DateTime.Today).TotalDays % daysInAWeek;
            }
            else
            {
                int daysSinceStartOfWindow = (int)(DateTime.Today - DueDate.Value.Date).TotalDays % daysInAWeek;
                daysUntilEndOfWindow = 7 - daysSinceStartOfWindow;
            }

            return DateTime.Today + TimeSpan.FromDays(daysUntilEndOfWindow) - TimeSpan.FromDays(daysInAWeek); ;
        }

        // Assessment 4 addition
        public override void SaveTo(BinaryWriter writer)
        {
            SaveUtils.SaveAndPrintInt(writer, 2);    // Indicator for which type of Task this is. 2 = "Habit"
            SaveDataTo(writer);
        }

        protected new void SaveDataTo(BinaryWriter writer)
        {
            base.SaveDataTo(writer);

            SaveUtils.SaveAndPrintInt(writer, Streak);
            SaveUtils.SaveAndPrintLong(writer, LastDueDateSatisfied == null ? 0 : LastDueDateSatisfied.Value.Ticks);
        }

        /// <summary>
        /// Destructively attempted a binary load.
        /// </summary>
        public override void LoadFrom(BinaryReader reader)
        {
            base.LoadFrom(reader);

            Streak = SaveUtils.LoadAndPrintInt(reader);

            long lastDueDateSatisfiedTicks = SaveUtils.LoadAndPrintLong(reader);
            LastDueDateSatisfied = lastDueDateSatisfiedTicks == 0 ? null : new(lastDueDateSatisfiedTicks);
        }

        public override string ToString()
        {
            return $"Habit: {Description}\n";
        }
    }
}
