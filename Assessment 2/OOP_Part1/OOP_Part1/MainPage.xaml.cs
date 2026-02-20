using OOP_Part1.Models;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System;

namespace OOP_Part1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Testing content
        Task Task0 = new("Deliver papers");
        Task Task1 = new("Check flight activity");
        Task Task2 = new("Sweep");
        Task Task3 = new("Water plants");

        RepeatingTask RepeatingWeeklyTask0 = new("Deliver papers", DateTime.Now + TimeSpan.FromDays(7), Frequency.Weekly);
        RepeatingTask RepeatingWeeklyTask1 = new("Check flight activity", DateTime.Now + TimeSpan.FromDays(7), Frequency.Weekly);
        RepeatingTask RepeatingDailyTask2 = new("Sweep", DateTime.Now + TimeSpan.FromDays(1), Frequency.Daily);
        RepeatingTask RepeatingDailyTask3 = new("Water plants", DateTime.Now + TimeSpan.FromDays(1), Frequency.Daily);


        public MainPage()
        {
            InitializeComponent();

            TestTaskPriorities();
            TestTaskDueDates();
            TestRepeatingTasks();
            TestHabitStreaks();
            TestProjects();
        }
        
        private void TestTaskPriorities()
        {
            Debug.WriteLine("\n\n\n ============ TESTING TASK PRIORITIES ============ \n\n\n");

            Task0.TaskPriority++;
            Task0.TaskPriority++;
            Task0.TaskPriority++;
            Task0.TaskPriority++;
            Task0.TaskPriority++;
            Task0.TaskPriority++;
            Task1.TaskPriority--;
            Task1.TaskPriority--;
            Task1.TaskPriority--;
            Task1.TaskPriority--;
            Task2.TaskPriority++;
            Task2.TaskPriority--;
            Task2.TaskPriority++;

            Debug.WriteLine($"Task0 Priority: : {Task0.TaskPriority.Value}");
            Debug.WriteLine($"Task1 Priority: : {Task1.TaskPriority.Value}");
            Debug.WriteLine($"Task2 Priority: : {Task2.TaskPriority.Value}");
            Debug.WriteLine($"Task3 Priority: : {Task3.TaskPriority.Value}");
        }

        private void TestTaskDueDates()
        {
            Debug.WriteLine("\n\n\n ============ TESTING TASK DUE DATES ============ \n\n\n");

            // 4 tasks with due tasks, past and future
            Task0.DueDate = DateTime.Now + TimeSpan.FromHours(2);
            Task1.DueDate = DateTime.Now + TimeSpan.FromDays(-10);
            RepeatingDailyTask2.DueDate = DateTime.Now + TimeSpan.FromHours(-2);
            RepeatingDailyTask3.DueDate = DateTime.Now + TimeSpan.FromDays(10);

            // Now output their statuses, if any
            Debug.WriteLine($"Task0 is {((bool)Task0.Overdue? "" : "not ")}overdue");
            Debug.WriteLine($"Task1 is {((bool)Task1.Overdue ? "" : "not ")}overdue");
            Debug.WriteLine($"RepeatingDailyTask2 is {((bool)RepeatingDailyTask2.Overdue ? "" : "not ")}overdue");
            Debug.WriteLine($"RepeatingDailyTask3 is {((bool)RepeatingDailyTask3.Overdue ? "" : "not ")}overdue");

            Debug.WriteLine("");

            Debug.WriteLine($"Task0 {(Task0.Overdue == null? "doesn't have" : "has")} a due date.");
            Debug.WriteLine($"Task1 {(Task1.Overdue == null? "doesn't have" : "has")} a due date.");
            Debug.WriteLine($"Task2 {(Task2.Overdue == null ? "doesn't have" : "has")} a due date.");
            Debug.WriteLine($"Task3 {(Task3.Overdue == null ? "doesn't have" : "has")} a due date.");

            Debug.WriteLine($"RepeatingWeeklyTask0 {(RepeatingWeeklyTask0.Overdue == null ? "doesn't have" : "has")} a due date.");
            Debug.WriteLine($"RepeatingWeeklyTask1 {(RepeatingWeeklyTask1.Overdue == null ? "doesn't have" : "has")} a due date.");
            Debug.WriteLine($"RepeatingDailyTask2 {(RepeatingDailyTask2.Overdue == null ? "doesn't have" : "has")} a due date.");
            Debug.WriteLine($"RepeatingDailyTask3 {(RepeatingDailyTask3.Overdue == null ? "doesn't have" : "has")} a due date.");
        }

        private void TestRepeatingTasks()
        {
            Debug.WriteLine("\n\n\n ============ TESTING REPEATING TASKS ============ \n\n\n");

            RepeatingWeeklyTask0.DueDate = DateTime.Now + TimeSpan.FromDays(2);
            RepeatingWeeklyTask1.DueDate = DateTime.Now + TimeSpan.FromDays(-11);
            RepeatingDailyTask2.DueDate = DateTime.Now + TimeSpan.FromDays(-4);
            RepeatingDailyTask3.DueDate = DateTime.Now + TimeSpan.FromDays(11);

            Debug.WriteLine("After re-initialising all 4 repeating tasks:");
            Debug.WriteLine("(Due dates are as the user defines, even in the past.)\n");

            Debug.WriteLine($"RepeatingWeeklyTask0 is/was due: {RepeatingWeeklyTask0.DueDate}");
            Debug.WriteLine($"RepeatingWeeklyTask1 is/was due: {RepeatingWeeklyTask1.DueDate}");
            Debug.WriteLine($"RepeatingDailyTask2 is/was due: {RepeatingDailyTask2.DueDate}");
            Debug.WriteLine($"RepeatingDailyTask3 is/was due: {RepeatingDailyTask3.DueDate}");

            RepeatingWeeklyTask0.ToggleCompletion();
            RepeatingWeeklyTask1.ToggleCompletion();
            RepeatingDailyTask2.ToggleCompletion();
            RepeatingDailyTask3.ToggleCompletion();

            Debug.WriteLine("\nAfter completing all 4 repeating tasks once:");
            Debug.WriteLine("(Due dates are forwarded to the next valid future due date, based on the user's initial due date.)\n");

            Debug.WriteLine($"RepeatingWeeklyTask0 is now due: {RepeatingWeeklyTask0.DueDate}");
            Debug.WriteLine($"RepeatingWeeklyTask1 is now due: {RepeatingWeeklyTask1.DueDate}");
            Debug.WriteLine($"RepeatingDailyTask2 is now due: {RepeatingDailyTask2.DueDate}");
            Debug.WriteLine($"RepeatingDailyTask3 is now due: {RepeatingDailyTask3.DueDate}");

            Debug.WriteLine("\nAfter un-completing all 4 repeating tasks:");
            Debug.WriteLine("(Due dates are wound back to the next valid future due date, based on the user's initial due date.)\n");

            Debug.WriteLine($"RepeatingWeeklyTask0 is now due: {RepeatingWeeklyTask0.DueDate}");
            Debug.WriteLine($"RepeatingWeeklyTask1 is now due: {RepeatingWeeklyTask1.DueDate}");
            Debug.WriteLine($"RepeatingDailyTask2 is now due: {RepeatingDailyTask2.DueDate}");
            Debug.WriteLine($"RepeatingDailyTask3 is now due: {RepeatingDailyTask3.DueDate}");
        }

        private void TestHabitStreaks()
        {
            Debug.WriteLine("\n\n\n ============ TESTING HABIT STREAKS ============ \n\n\n");

            Debug.WriteLine("4 habit tasks are created, with initial dummy streak values and due dates hard-coded for demonstration.");
            Debug.WriteLine("Observe the due dates of these dummy habits to visualse which should and shouldn't have streaks");
            Debug.WriteLine("continued once they've been completed today.\n");

            Habit WeeklyHabit0 = new("Get gains", DateTime.Now + TimeSpan.FromDays(-77), Frequency.Weekly, 10);
            Habit WeeklyHabit1 = new("Get swol", DateTime.Now + TimeSpan.FromDays(3), Frequency.Weekly, 2);
            Habit DailyHabit2 = new("Beefcake", DateTime.Now + TimeSpan.FromDays(-57), Frequency.Daily, 14);
            Habit DailyHabit3 = new("Post memes", DateTime.Now + TimeSpan.FromDays(1), Frequency.Daily, 55);

            Debug.WriteLine($"Habit0 initialised with a dummy streak of: {WeeklyHabit0.Streak}, due: {WeeklyHabit0.DueDate}.");
            Debug.WriteLine($"Habit1 initialised with a dummy streak of: {WeeklyHabit1.Streak}, due: {WeeklyHabit1.DueDate}.");
            Debug.WriteLine($"Habit2 initialised with a dummy streak of: {DailyHabit2.Streak}, due: {DailyHabit2.DueDate}.");
            Debug.WriteLine($"Habit3 initialised with a dummy streak of: {DailyHabit3.Streak}, due: {DailyHabit3.DueDate}.");

            Debug.WriteLine("\nUpon completing all 4 tasks, any that were due more than 1 week / 1 day in the past will have reset their streak back to 1.");
            Debug.WriteLine("And any that were due in a current due date window will have incremented streaks.\n");

            WeeklyHabit0.ToggleCompletion();
            WeeklyHabit1.ToggleCompletion();
            DailyHabit2.ToggleCompletion();
            DailyHabit3.ToggleCompletion();

            Debug.WriteLine($"Habit0 now with a dummy streak of: {WeeklyHabit0.Streak}, due: {WeeklyHabit0.DueDate}.");
            Debug.WriteLine($"Habit1 now with a dummy streak of: {WeeklyHabit1.Streak}, due: {WeeklyHabit1.DueDate}.");
            Debug.WriteLine($"Habit2 now with a dummy streak of: {DailyHabit2.Streak}, due: {DailyHabit2.DueDate}.");
            Debug.WriteLine($"Habit3 now with a dummy streak of: {DailyHabit3.Streak}, due: {DailyHabit3.DueDate}.");

            Debug.WriteLine("\nNow we'll un-complete those tasks. We should see streak counters reduce for everything, and we should see");
            Debug.WriteLine("due dates adjust to a valid near-future due date that's been fast-forwarded from the initial dummy due date.\n");

            WeeklyHabit0.ToggleCompletion();
            WeeklyHabit1.ToggleCompletion();
            DailyHabit2.ToggleCompletion();
            DailyHabit3.ToggleCompletion();

            Debug.WriteLine($"Habit0 now with a dummy streak of: {WeeklyHabit0.Streak}, due: {WeeklyHabit0.DueDate}.");
            Debug.WriteLine($"Habit1 now with a dummy streak of: {WeeklyHabit1.Streak}, due: {WeeklyHabit1.DueDate}.");
            Debug.WriteLine($"Habit2 now with a dummy streak of: {DailyHabit2.Streak}, due: {DailyHabit2.DueDate}.");
            Debug.WriteLine($"Habit3 now with a dummy streak of: {DailyHabit3.Streak}, due: {DailyHabit3.DueDate}.");

            Debug.WriteLine("\nHuzzah. The only place in the code that sees dummy values is the Habit ctor.");
            Debug.WriteLine("Check its comment  for further explanation of the dummy variables.\n");
        }

        private void TestProjects()
        {
            Debug.WriteLine("\n\n\n ============ TESTING PROJECTS ============ \n\n\n");

            Project timeProject = new("Time Machine Build 1");
            Habit ponderKittensHabit = new("Ponder kittens", DateTime.Today, Frequency.Weekly, 0);

            timeProject.AddTask(Task0);
            timeProject.AddTask(Task1);
            timeProject.AddTask(Task2);
            timeProject.AddTask(Task3);
            timeProject.AddTask(RepeatingWeeklyTask1);
            timeProject.AddTask(ponderKittensHabit);

            Task0.ToggleCompletion();
            Task1.ToggleCompletion();
            Task3.ToggleCompletion();

            Debug.WriteLine($"\nAfter completing 3 of the project's 4 tasks, the project is {timeProject.PercentComplete}% complete.\n");
        }
    }
}
