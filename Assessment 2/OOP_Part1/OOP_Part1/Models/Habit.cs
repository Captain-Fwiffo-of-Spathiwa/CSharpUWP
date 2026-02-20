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

        private int             Streak = 0;
        private DateTime?       LastDueDateSatisfied = null;

        public Habit(string description, DateTime dueDate, Frequency frequency)
            : base(description, dueDate, frequency)
        { }

        public override void ToggleCompletion()
        {
            // Toggle the task's completion status via the parent
            base.ToggleCompletion();

            DateTime previousDueDate = (DateTime)DueDate - TimeSpan.FromDays(Frequency == Frequency.Daily ? 1 : 7);

            // If we have just completed an incomplete task, see if we're
            // still on our streak.
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
            // If we have un-completed a task that had been completed, just
            // wind back the streak counter and tracked due date.
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
