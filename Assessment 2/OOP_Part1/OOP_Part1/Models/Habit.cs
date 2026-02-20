using System;



namespace OOP_Part1.Models
{
    internal class Habit : RepeatingTask
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
            DateLastCompleted = dueDate - TimeSpan.FromDays(Frequency == Frequency.Daily ? 1 : 7);
            LastDueDateSatisfied = dueDate - TimeSpan.FromDays(Frequency == Frequency.Daily ? 1 : 7);
        }

        public override void ToggleCompletion()
        {
            // Toggle the task's completion status via the parent
            base.ToggleCompletion();

            DateTime previousDueDate = (DateTime)DueDate - TimeSpan.FromDays(Frequency == Frequency.Daily ? 1 : 7);

            // If we have just completed a task, we're still on our streak
            // *as long as* the last due date satisfied was in the previous
            // due date cycle, and not far in the past.
            if (IsComplete)
            {
                if (LastDueDateSatisfied == previousDueDate)
                {
                    ++Streak;
                }
                else
                {
                    Streak = 1;
                }

                LastDueDateSatisfied = DueDate;
            }
            // If we have un-completed a task that had been completed, we need
            // to wind back the streak counter and the tracked due date.
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
    }
}
