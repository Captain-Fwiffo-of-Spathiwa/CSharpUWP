using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Windows.Storage;
using TaskManagement.Helpers;
using TaskManagement.Models;
using System.Collections.Generic;
using System.Linq;



namespace UnitTests
{
    /// <summary>
    /// Unit tests for TaskCollections, TaskLists, Tasks, and their subtypes.
    /// Last run: March 23rd, 2026
    /// 
    /// Changelog:
    /// ----------
    /// 10/05/26: Add remaining natural date tests.
    /// 01/05/26: Add initial natural date tests.
    /// 24/03/26: Fix RepeatingTask completion timings.
    /// 23/03/26: Collection, List, and Task tests added. Fixed TestSaveAndLoadTaskCollection().
    /// 16/03/06: Binary save and load tests added.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        #region Binary saving test members

        string TestBinarySaveFilename;
        StorageFolder TestFolder;
        StorageFile TestFile;

        TaskManagement.Models.Task TaskA;
        TaskManagement.Models.Task TaskB;
        TaskManagement.Models.RepeatingTask RepTaskA;
        TaskManagement.Models.RepeatingTask RepTaskB;
        TaskManagement.Models.Habit HabitA;
        TaskManagement.Models.Habit HabitB;

        #endregion


        #region Assessment 3 test members

        TaskManagement.Models.TaskList AT3TasksListA;
        TaskManagement.Models.TaskList AT3TasksListB;
        TaskManagement.Models.TaskCollection AT3TasksCollection;

        #endregion


        #region Assessment 4 test members

        TaskManagement.Models.TaskList AT4TasksListA;
        TaskManagement.Models.TaskList AT4TasksListB;
        TaskManagement.Models.TaskCollection AT4TasksCollection;

        #endregion


        #region Assessment 5 test members

        TaskCollection NoListsCollection;
        TaskCollection NoTasksButListsCollection;
        TaskCollection TasksInSomeListsCollection;
        TaskCollection AllListsHaveTasksWithDuplicatesCollection;

        TaskList EmptyListA;
        TaskList EmptyListB;
        //TaskList ListWithEmptyTasksA;
        //TaskList ListWithEmptyTasksB;
        TaskList ListWithTasksAndSubTypes;
        TaskList ListWithTaskSubType;

        Task TaskForSorting1;
        Task TaskForSorting2;
        Task TaskForSorting3;
        Task TaskForSorting4;
        Habit TaskForSorting5;
        Habit TaskForSorting6;
        Habit TaskForSorting7;
        Habit TaskForSorting8;
        RepeatingTask TaskForSorting9;
        RepeatingTask TaskForSorting10;
        RepeatingTask TaskForSorting11;
        RepeatingTask TaskForSorting12;

        #endregion


        #region Assessment 7 test members

        // Every test uses a new string, so no generalised setup here.

        #endregion


        /// <summary>
        /// Setup class fields used by our tests.
        /// </summary>
        [TestInitialize]
        public async System.Threading.Tasks.Task Setup()
        {
            /* ------------------------------------------------------------
                A constructor could do this work too, but [TestInitialize]
                means we get exceptions handled by reporting them as test
                failures.
               -----------------------------------------------------------*/

            #region Binary saving test member setup

            /* ------------------------------------------------------------
                We want this file creation to be async, because the code
                being tested uses async. With UnitTests, async functions
                need to return a Task so that the test framework can
                await/work with those Tasks properly.
                
                Yes, we have our own type called "Task".
                That's just unfortunate.
               -----------------------------------------------------------*/

            TestBinarySaveFilename = "TestBinarySaveFile.bin";
            TestFolder = ApplicationData.Current.LocalFolder;
            TestFile = await TestFolder.CreateFileAsync(TestBinarySaveFilename, CreationCollisionOption.ReplaceExisting);

            TaskA = new("Buy cats");
            TaskB = new("Buy dogs");
            RepTaskA = new("Tend to cats", DateTime.Now, TaskManagement.Models.Frequency.Weekly);
            RepTaskB = new("Tend to dogs", DateTime.Now, TaskManagement.Models.Frequency.Daily);
            HabitA = new("Pat dogs", DateTime.Now, TaskManagement.Models.Frequency.Daily, 5);
            HabitB = new("Ignore cats", DateTime.Now, TaskManagement.Models.Frequency.Daily, 10);

            #endregion


            #region Assessment 3 test member setup

            AT3TasksListA = new("New list 1");
            AT3TasksListB = new("new list 2");

            AT3TasksCollection = new();

            #endregion


            #region Assessment 4 test member setup

            AT4TasksListA = new("List of Tasks");
            AT4TasksListB = new("Other list of Tasks");

            AT4TasksCollection = new();

            AT4TasksListA.AddTask(TaskA);
            AT4TasksListA.AddTask(RepTaskA);
            AT4TasksListA.AddTask(HabitA);

            AT4TasksListB.AddTask(TaskB);
            AT4TasksListB.AddTask(RepTaskB);
            AT4TasksListB.AddTask(HabitB);

            AT4TasksCollection.AddTaskList(AT4TasksListA);
            AT4TasksCollection.AddTaskList(AT4TasksListB);

            Assert.AreEqual(AT4TasksListA.TotalTasksCount, 3);
            Assert.AreEqual(AT4TasksListB.TotalTasksCount, 3);

            Assert.AreEqual(AT4TasksCollection.TotalTasksCount, 6);
            Assert.AreEqual(AT4TasksCollection.IncompleteTasksCount, 6);

            #endregion


            #region Assessment 5 test member setup

            // Create our 12 Tasks. Make sure none use DateTime.Now for testing consistency.
            TaskForSorting1 = new("Always bake");                                                                       // Due 1st      Created first
            TaskForSorting1.DueDate = DateTime.Today + TimeSpan.FromDays(-1000);
            TaskForSorting1.TaskPriority.Value = 90;
            TaskForSorting2 = new("Bake cakes");                                                                        // Due 5th
            TaskForSorting2.DueDate = DateTime.Today;
            TaskForSorting2.TaskPriority.Value = 80;
            TaskForSorting3 = new("Cakes to decorate");                                                                 // Due 6th
            TaskForSorting3.DueDate = DateTime.Today;
            TaskForSorting3.TaskPriority.Value = 70;
            TaskForSorting4 = new("Decorate before eating");                                                            // Due 7th
            TaskForSorting4.DueDate = DateTime.Today + TimeSpan.FromDays(1);
            TaskForSorting4.TaskPriority.Value = 60;
            TaskForSorting5 = new("Eat every week", DateTime.Today + TimeSpan.FromDays(4), Frequency.Weekly);           // Due 9th
            TaskForSorting5.TaskPriority.Value = 50;
            TaskForSorting6 = new("Fry the cakes", DateTime.Today + TimeSpan.FromDays(2), Frequency.Daily);             // Due 8th                      Highest priority
            TaskForSorting6.TaskPriority.Value = 40;
            TaskForSorting7 = new("Grate the frypans", DateTime.Today + TimeSpan.FromDays(-100), Frequency.Weekly);     // Due 2nd
            TaskForSorting7.TaskPriority.Value = 45;
            TaskForSorting8 = new("Heat the grater", DateTime.Today + TimeSpan.FromDays(77), Frequency.Daily);          // Due 12th
            TaskForSorting8.TaskPriority.Value = 55;
            TaskForSorting9 = new("Ice the heater", DateTime.Today + TimeSpan.FromDays(-88), Frequency.Weekly);         // Due 3rd
            TaskForSorting9.TaskPriority.Value = 65;
            TaskForSorting10 = new("Jump the Kiwi", DateTime.Today + TimeSpan.FromDays(8), Frequency.Daily);            // Due 10th
            TaskForSorting10.TaskPriority.Value = 75;
            TaskForSorting11 = new("Kiwi, who's that??", DateTime.Today + TimeSpan.FromDays(-8), Frequency.Weekly);     // Due 4th
            TaskForSorting11.TaskPriority.Value = 85;
            TaskForSorting12 = new("Quick Kiwi! Get 'im!", DateTime.Today + TimeSpan.FromDays(55), Frequency.Daily);    // Due 11th     Created last    Lowest priority
            TaskForSorting12.TaskPriority.Value = 95;

            // Make and populate 4 TaskLists
            EmptyListA = new("EmptyListA");

            EmptyListB = new("EmptyListB");

            ListWithTasksAndSubTypes = new("ListWithTasksAndSubTypesA");
            ListWithTasksAndSubTypes.AddTask(TaskForSorting1);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting2);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting3);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting4);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting5);
            // Deliberately skip some another list will have out of order
            ListWithTasksAndSubTypes.AddTask(TaskForSorting8);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting9);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting10);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting11);
            ListWithTasksAndSubTypes.AddTask(TaskForSorting12);

            ListWithTaskSubType = new("ListWithTasksAndSubTypesB");
            ListWithTaskSubType.AddTask(TaskForSorting5);
            ListWithTaskSubType.AddTask(TaskForSorting6);
            ListWithTaskSubType.AddTask(TaskForSorting7);
            ListWithTaskSubType.AddTask(TaskForSorting8);

            // Make TaskCollections from those TaskLists with different combos
            // of populated / not-populated.
            NoListsCollection = new();

            NoTasksButListsCollection = new();
            NoTasksButListsCollection.AddTaskList(EmptyListA);
            NoTasksButListsCollection.AddTaskList(EmptyListB);

            TasksInSomeListsCollection = new();
            TasksInSomeListsCollection.AddTaskList(EmptyListA);
            TasksInSomeListsCollection.AddTaskList(ListWithTasksAndSubTypes);

            AllListsHaveTasksWithDuplicatesCollection = new();
            AllListsHaveTasksWithDuplicatesCollection.AddTaskList(ListWithTasksAndSubTypes);
            AllListsHaveTasksWithDuplicatesCollection.AddTaskList(ListWithTaskSubType);

            #endregion


            #region Assessment 7 test member setup

            // Every test uses a new string, so no generalised setup here.

            #endregion

        }


        #region Unit testing

        /// <summary>
        /// Test adding new TaskLists to a Collection.
        /// </summary>
        [TestMethod]
        public void TestAddNewListToCollection()
        {
            Assert.AreEqual(0, AT3TasksCollection.TotalTasksCount);
            Assert.AreEqual("\n ------------------------ \n", AT3TasksCollection.ToString());

            // Empty list
            AT3TasksCollection.AddTaskList(AT3TasksListA);
            Assert.AreEqual(0, AT3TasksCollection.TotalTasksCount);

            // Populated list
            AT3TasksListA.AddTask(TaskA);
            Assert.AreEqual(1, AT3TasksCollection.TotalTasksCount);

            // Collection sees updates to the added TaskList
            AT3TasksListB.AddTask(TaskA);
            AT3TasksListB.AddTask(HabitA);
            AT3TasksCollection.AddTaskList(AT3TasksListB);
            Assert.AreEqual(3, AT3TasksCollection.TotalTasksCount);

            String expectedString = "Task: Buy cats\nTask: Buy cats\nHabit: Pat dogs\n\n ------------------------ \n";
            Assert.AreEqual(expectedString, AT3TasksCollection.ToString());
        }

        /// <summary>
        /// Test adding Tasks to a TaskList.
        /// </summary>
        [TestMethod]
        public void TestAddTasksToList()
        {
            Assert.AreEqual(0, AT3TasksListA.TotalTasksCount);

            // Add Task
            AT3TasksListA.AddTask(TaskA);
            Assert.AreEqual(1, AT3TasksListA.TotalTasksCount);

            // Add duplicate Task
            AT3TasksListA.AddTask(TaskA);
            Assert.AreEqual(2, AT3TasksListA.TotalTasksCount);

            // Add Task derived types
            AT3TasksListA.AddTask(RepTaskA);
            AT3TasksListA.AddTask(HabitB);
            Assert.AreEqual(4, AT3TasksListA.TotalTasksCount);

            String expected = "Task: Buy cats\nTask: Buy cats\nRepeating Task: Tend to cats\nHabit: Ignore cats\n";
            Assert.AreEqual(expected, AT3TasksListA.ToString());
        }

        /// <summary>
        /// Test completing Tasks.
        /// </summary>
        [TestMethod]
        public void TestCompleteTasks()
        {
            Assert.AreEqual(0, AT3TasksListA.IncompleteTasksCount);

            // Setup
            AT3TasksListA.AddTask(TaskA);
            AT3TasksListA.AddTask(TaskB);
            AT3TasksListA.AddTask(RepTaskA);
            AT3TasksListA.AddTask(RepTaskB);
            AT3TasksListA.AddTask(HabitA);
            AT3TasksListA.AddTask(HabitB);
            Assert.IsFalse(TaskA.IsComplete);
            Assert.IsFalse(TaskB.IsComplete);
            Assert.IsFalse(RepTaskA.IsComplete);
            Assert.IsFalse(RepTaskB.IsComplete);
            Assert.IsFalse(HabitA.IsComplete);
            Assert.IsFalse(HabitB.IsComplete);
            Assert.AreEqual(6, AT3TasksListA.IncompleteTasksCount);
            Assert.AreEqual(6, AT3TasksListA.TotalTasksCount);
            TaskA.ToggleCompletion();
            RepTaskA.ToggleCompletion();

            // Check individial completion
            Assert.IsTrue(TaskA.IsComplete);
            Assert.IsTrue(RepTaskA.IsComplete);

            // Check TaskList counts for completion
            Assert.AreEqual(4, AT3TasksListA.IncompleteTasksCount);
            Assert.AreEqual(6, AT3TasksListA.TotalTasksCount);
            //Assert.AreEqual(2, AT3TasksListA.TotalTasksCount - AT3TasksListA.IncompleteTasksCount);

            // Perform operation on only complete Tasks and check result
            AT3TasksListA.ClearCompletedTasks();
            Assert.AreEqual(4, AT3TasksListA.TotalTasksCount);
        }

        /// <summary>
        /// Test setting complete tasks as incomplete.
        /// </summary>
        [TestMethod]
        public void TestSetTaskIncomplete()
        {
            Assert.AreEqual(0, AT3TasksListA.IncompleteTasksCount);

            // Setup
            AT3TasksListA.AddTask(TaskA);
            AT3TasksListA.AddTask(RepTaskA);
            AT3TasksListA.AddTask(HabitA);
            TaskA.ToggleCompletion();
            RepTaskA.ToggleCompletion();
            HabitA.ToggleCompletion();
            Assert.IsTrue(TaskA.IsComplete);
            Assert.IsTrue(RepTaskA.IsComplete);
            Assert.IsTrue(HabitA.IsComplete);
            TaskA.ToggleCompletion();
            RepTaskA.ToggleCompletion();

            // Check individual incompletion
            Assert.IsFalse(TaskA.IsComplete);
            Assert.IsFalse(RepTaskA.IsComplete);

            // Check TaskList counts
            Assert.AreEqual(2, AT3TasksListA.IncompleteTasksCount);

            // Perform operation that ignores only incomplete Tasks and check result
            Assert.AreEqual(3, AT3TasksListA.TotalTasksCount);
            AT3TasksListA.ClearCompletedTasks();
            Assert.AreEqual(2, AT3TasksListA.TotalTasksCount);
        }

        /// <summary>
        /// Test removing tasks via completion.
        /// </summary>
        [TestMethod]
        public void TestDeleteTasksFromList()
        {
            Assert.AreEqual(0, AT3TasksListA.IncompleteTasksCount);

            AT3TasksListA.AddTask(TaskA);
            AT3TasksListA.AddTask(TaskB);
            AT3TasksListA.AddTask(RepTaskA);
            AT3TasksListA.AddTask(RepTaskB);
            AT3TasksListA.AddTask(HabitA);
            AT3TasksListA.AddTask(HabitB);
            Assert.AreEqual(6, AT3TasksListA.IncompleteTasksCount);
            Assert.AreEqual(6, AT3TasksListA.TotalTasksCount);

            TaskA.ToggleCompletion();
            TaskB.ToggleCompletion();
            AT3TasksListA.ClearCompletedTasks();
            Assert.AreEqual(4, AT3TasksListA.TotalTasksCount);

            RepTaskA.ToggleCompletion();
            RepTaskB.ToggleCompletion();
            HabitA.ToggleCompletion();
            HabitB.ToggleCompletion();
            AT3TasksListA.ClearCompletedTasks();
            Assert.AreEqual(0, AT3TasksListA.TotalTasksCount);

            AT3TasksListA.ClearCompletedTasks();
            Assert.AreEqual(0, AT3TasksListA.TotalTasksCount);
        }

        /// <summary>
        /// Test that RepeatingTasks repeat correctly.
        /// </summary>
        [TestMethod]
        public void TestRepeatingTasksRepeat()
        {
            /* ------------------------------------------------------------ 
             *  A RepeatingTask repeats by updating its DueDate in
             *  response to completion and its existing DueDate. If the
             *  RepeatingTask's existing due date window wraps the present
             *  day or is in the past, that DueDate is updated to bookend
             *  the next due date window in the future.
             * -----------------------------------------------------------*/

            // Setup
            String tomorrow = (DateTime.Now + TimeSpan.FromDays(1)).ToShortDateString();

            // Check recently overdue tasks
            // Daily 
            {
                int numDaysAgoForYesterday = 1;
                RepeatingTask recentlyOverdueDailyTask = new RepeatingTask("recentlyOverdueDailyTask", DateTime.Now - TimeSpan.FromDays(numDaysAgoForYesterday), Frequency.Daily);
                recentlyOverdueDailyTask.ToggleCompletion();
                Assert.IsTrue(recentlyOverdueDailyTask.IsComplete);
                Assert.AreEqual(tomorrow, recentlyOverdueDailyTask.DueDate.Value.ToShortDateString());
            }
            // Weekly
            {
                int numDaysAgoForThisWeek = 4;  // A value that's between 0 and 1 weeks ago
                RepeatingTask recentlyOverdueWeeklyTask = new RepeatingTask("recentlyOverdueWeeklyTask", DateTime.Now - TimeSpan.FromDays(numDaysAgoForThisWeek), Frequency.Weekly);
                recentlyOverdueWeeklyTask.ToggleCompletion();
                Assert.IsTrue(recentlyOverdueWeeklyTask.IsComplete);
                int numDaysIn2Weeks = 14;
                String nextWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(numDaysIn2Weeks - numDaysAgoForThisWeek)).ToShortDateString();
                Assert.AreEqual(nextWeeklyDueDate, recentlyOverdueWeeklyTask.DueDate.Value.ToShortDateString());
            }

            // Check long overdue tasks
            // Daily
            {
                int numDaysAgoForDistantDay = 3;
                RepeatingTask longOverdueDailyTask = new RepeatingTask("longOverdueDueDailyTask", DateTime.Now - TimeSpan.FromDays(numDaysAgoForDistantDay), Frequency.Daily);
                longOverdueDailyTask.ToggleCompletion();
                Assert.IsTrue(longOverdueDailyTask.IsComplete);
                Assert.AreEqual(tomorrow, longOverdueDailyTask.DueDate.Value.ToShortDateString());
            }
            // Weekly
            {
                int numDaysAgoForDistantWeek = 11;  // A value that's between 1 and 2 weeks ago
                RepeatingTask longOverdueWeeklyTask = new RepeatingTask("longOverdueWeeklyTask", DateTime.Now - TimeSpan.FromDays(numDaysAgoForDistantWeek), Frequency.Weekly);
                longOverdueWeeklyTask.ToggleCompletion();
                Assert.IsTrue(longOverdueWeeklyTask.IsComplete);
                int daysIn3Weeks = 21;
                String nextWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(daysIn3Weeks - numDaysAgoForDistantWeek)).ToShortDateString();
                Assert.AreEqual(nextWeeklyDueDate, longOverdueWeeklyTask.DueDate.Value.ToShortDateString());
            }

            // Check currently due tasks
            // Daily
            {
                int numDaysAheadForTomorrow = 1;
                RepeatingTask currentlyDueDailyTask = new RepeatingTask("currentlyDueDailyTask", DateTime.Now + TimeSpan.FromDays(numDaysAheadForTomorrow), Frequency.Daily);
                currentlyDueDailyTask.ToggleCompletion();
                Assert.IsTrue(currentlyDueDailyTask.IsComplete);
                Assert.AreEqual(tomorrow, currentlyDueDailyTask.DueDate.Value.ToShortDateString());
            }
            // Weekly
            {
                int numDaysAheadForThisWeek = 5;    // A value that's between 0 and 1 weeks in the future
                RepeatingTask currentlyDueWeeklyTask = new RepeatingTask("currentlyDueWeeklyTask", DateTime.Now + TimeSpan.FromDays(numDaysAheadForThisWeek), Frequency.Weekly);
                currentlyDueWeeklyTask.ToggleCompletion();
                Assert.IsTrue(currentlyDueWeeklyTask.IsComplete);
                int daysIn1Week = 7;
                String nextWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(daysIn1Week + numDaysAheadForThisWeek)).ToShortDateString();
                Assert.AreEqual(nextWeeklyDueDate, currentlyDueWeeklyTask.DueDate.Value.ToShortDateString());
            }

            // Check eventually due tasks
            // Daily
            {
                int numDaysAheadForDistantDay = 4;
                RepeatingTask eventuallyDueDailyTask = new RepeatingTask("eventuallyDueDailyTask", DateTime.Now + TimeSpan.FromDays(numDaysAheadForDistantDay), Frequency.Daily);
                eventuallyDueDailyTask.ToggleCompletion();
                Assert.IsTrue(eventuallyDueDailyTask.IsComplete);

                // Ensure our distant due task wasn't set to be due tomorrow
                String eventualDailyDueDate = (DateTime.Now + TimeSpan.FromDays(numDaysAheadForDistantDay)).ToShortDateString();
                Assert.AreEqual(eventualDailyDueDate, eventuallyDueDailyTask.DueDate.Value.ToShortDateString());
            }
            // Weekly
            {
                int numDaysAheadForDistantWeek = 12;    // A value that's between 1 and 2 weeks in the future
                RepeatingTask eventuallyDueWeeklyTask = new RepeatingTask("eventuallyDueWeeklyTask", DateTime.Now + TimeSpan.FromDays(numDaysAheadForDistantWeek), Frequency.Weekly);
                eventuallyDueWeeklyTask.ToggleCompletion();
                Assert.IsTrue(eventuallyDueWeeklyTask.IsComplete);

                // Ensure our distant due task wasn't set to be due next week
                String eventualWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(numDaysAheadForDistantWeek)).ToShortDateString();
                Assert.AreEqual(eventualWeeklyDueDate, eventuallyDueWeeklyTask.DueDate.Value.ToShortDateString());
            }
        }

        /// <summary>
        /// Test that Habits correctly count completions and un-completions.
        /// </summary>
        [TestMethod]
        public void TestHabitsCountCompletions()
        {
            // Task due today with no streak
            {
                int startingStreak = 0;
                int expectedStreak = 1;
                Habit dailyNewStreak = new("dailyNewStreak", DateTime.Now, TaskManagement.Models.Frequency.Daily, startingStreak);
                dailyNewStreak.ToggleCompletion();
                Assert.AreEqual(expectedStreak, dailyNewStreak.Streak);

                // Un-complete the task and check the streak unwinds
                dailyNewStreak.ToggleCompletion();
                Assert.AreEqual(startingStreak, dailyNewStreak.Streak);
            }

            // Task due today with a running streak
            {
                int startingStreak = 2;
                int expectedStreak = 3;
                Habit dailyContinuedStreak = new("dailyContinuedStreak", DateTime.Now, TaskManagement.Models.Frequency.Daily, startingStreak);
                dailyContinuedStreak.ToggleCompletion();
                Assert.AreEqual(expectedStreak, dailyContinuedStreak.Streak);

                dailyContinuedStreak.ToggleCompletion();
                Assert.AreEqual(startingStreak, dailyContinuedStreak.Streak);
            }

            // Overdue daily task that had a streak
            {
                int startingStreak = 3;
                int expectedStreak = 1;
                Habit dailyBreakingStreak = new("dailyBreakingStreak", DateTime.Now - TimeSpan.FromDays(3), TaskManagement.Models.Frequency.Daily, startingStreak);
                dailyBreakingStreak.ToggleCompletion();
                Assert.AreEqual(expectedStreak, dailyBreakingStreak.Streak);

                dailyBreakingStreak.ToggleCompletion();
                Assert.AreEqual(0, dailyBreakingStreak.Streak); // Ensure un-completing doesn't return a streak that was already broken
            }

            // Task due this week with no streak
            {
                int startingStreak = 0;
                int expectedStreak = 1;
                Habit weeklyNewStreak = new("weeklyNewStreak", DateTime.Now + TimeSpan.FromDays(2), TaskManagement.Models.Frequency.Weekly, startingStreak);
                weeklyNewStreak.ToggleCompletion();
                Assert.AreEqual(expectedStreak, weeklyNewStreak.Streak);

                weeklyNewStreak.ToggleCompletion();
                Assert.AreEqual(startingStreak, weeklyNewStreak.Streak);
            }

            // Task due this week with a running streak
            {
                int startingStreak = 17;
                int expectedStreak = 18;
                Habit weeklyContinuedStreak = new("weeklyContinuedStreak", DateTime.Now + TimeSpan.FromDays(4), TaskManagement.Models.Frequency.Weekly, startingStreak);
                weeklyContinuedStreak.ToggleCompletion();
                Assert.AreEqual(expectedStreak, weeklyContinuedStreak.Streak);

                weeklyContinuedStreak.ToggleCompletion();
                Assert.AreEqual(startingStreak, weeklyContinuedStreak.Streak);
            }

            // Overdue weekly task that had a streak
            {
                int startingStreak = 99;
                int expectedStreak = 1;
                Habit weeklyBreakingStreak = new("weeklyBreakingStreak", DateTime.Now - TimeSpan.FromDays(3), TaskManagement.Models.Frequency.Weekly, startingStreak);
                weeklyBreakingStreak.ToggleCompletion();
                Assert.AreEqual(expectedStreak, weeklyBreakingStreak.Streak);

                weeklyBreakingStreak.ToggleCompletion();
                Assert.AreEqual(0, weeklyBreakingStreak.Streak);
            }
        }

        [TestMethod]
        public void TestProjectsCompletePercentage()
        {
            // No complete tasks
            Project testProject = new("testProject");
            testProject.AddTask(TaskA);
            testProject.AddTask(TaskB);
            Assert.AreEqual((int)testProject.PercentComplete, 0);

            // 1 of 2 tasks complete
            TaskA.ToggleCompletion();
            Assert.AreEqual((int)testProject.PercentComplete, 50);

            // Adding 2 more competed tasks
            Task newTaskA = new("pat the plants");
            Task newTaskB = new("water the dogs");
            newTaskA.ToggleCompletion();
            newTaskB.ToggleCompletion();
            testProject.AddTask(newTaskA);
            testProject.AddTask(newTaskB);
            Assert.AreEqual((int)testProject.PercentComplete, 75);

            // Un-completing those added tasks
            newTaskA.ToggleCompletion();
            newTaskB.ToggleCompletion();
            Assert.AreEqual((int)testProject.PercentComplete, 25);
        }

        #endregion


        #region Error testing

        /// <summary>
        /// Test the app handles setting a Task name to be blank.
        /// </summary>
        [TestMethod]
        public void TestBadTaskName()
        {
            // Creating a Task with a bad name
            try
            {
                Task task1 = new("");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid description given for new Task.", ex.Message);
            }

            // Setting a bad name on an existing Task
            Task task2 = new("task2");
            task2.SetDescription("");
            Assert.AreEqual("task2", task2.GetDescription());
        }

        /// <summary>
        /// Test the app handles setting a TaskList name to be blank.
        /// </summary>
        [TestMethod]
        public void TestBadListName()
        {
            // Creating a TaskList with a bad name
            try
            {
                TaskList tasklist1 = new("");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid description given for new TaskList.", ex.Message);
            }

            // Setting a bad name on an existing TaskList
            TaskList tasklist2 = new("tasklist2");
            tasklist2.SetName("");
            Assert.AreEqual("tasklist2", tasklist2.GetName());
        }

        /// <summary>
        /// Test the app handles attempting to place RepeatingTasks in a Project.
        /// </summary>
        [TestMethod]
        public void TestAddRepeatingTaskToProject()
        {
            Project testRepTasksProject = new("testRepTasksProject");
            Assert.AreEqual(0, testRepTasksProject.TotalTasksCount);

            testRepTasksProject.AddTask(RepTaskA);
            testRepTasksProject.AddTask(RepTaskB);
            Assert.AreEqual(0, testRepTasksProject.TotalTasksCount);
        }

        /// <summary>
        /// Test the app handles attempting to place Habits in a Project.
        /// </summary>
        [TestMethod]
        public void TestAddHabitToProject()
        {
            Project testHabitsProject = new("testHabitsProject");
            Assert.AreEqual(0, testHabitsProject.TotalTasksCount);

            testHabitsProject.AddTask(HabitA);
            testHabitsProject.AddTask(HabitB);
            Assert.AreEqual(0, testHabitsProject.TotalTasksCount);
        }

        /// <summary>
        /// Test the app handles a a RepeatingTask with incomplete information.
        /// </summary>
        [TestMethod]
        public void TestRepeatingTaskWithMissingInformation()
        {
            // RepeatingTasks can't be constructed with missing DueDate or
            // Frequency, but we can test a missing description.
            try
            {
                RepeatingTask repeatingTask1 = new("", DateTime.Now, Frequency.Daily);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid description given for new Task.", ex.Message);
            }

            RepeatingTask repeatingTask = new("repeatingTask", DateTime.Now, Frequency.Daily);
            repeatingTask.SetDescription("");
            Assert.AreEqual("repeatingTask", repeatingTask.GetDescription());
        }

        #endregion


        #region Assessment 4 Tests - Binary Files

        /// <summary>
        /// Test binary save and load of all Task subtypes.
        /// 
        /// The file used is overwritten and re-created for each test.
        /// </summary>
        [TestMethod]
        public void TestBinarySaveAndLoadTask()
        {
            TestBinarySaveAndLoadGivenTaskType(TaskA);
            TestBinarySaveAndLoadGivenTaskType(TaskB);
            TestBinarySaveAndLoadGivenTaskType(RepTaskA);
            TestBinarySaveAndLoadGivenTaskType(RepTaskB);
            TestBinarySaveAndLoadGivenTaskType(HabitA);
            TestBinarySaveAndLoadGivenTaskType(HabitB);
        }

        private void TestBinarySaveAndLoadGivenTaskType(TaskManagement.Models.Task task)
        {
            using (var stream = File.Open(TestFile.Path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    // Use polymorphism to save
                    task.SaveTo(writer);
                }
            }

            using (var stream = File.Open(TestFolder.Path + "\\" + TestBinarySaveFilename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    /* ----------------------------------------------------
                        We could binary load back into the received task
                        object, but that wouldn't make for a very smart
                        test. We want a whole new object instead so we're
                        sure the tested data is the loaded data. This
                        means we need to know the object's subtype.
                       ---------------------------------------------------*/

                    TaskManagement.Models.Task newTask;
                    int newTaskExpectedTypeID;

                    // The order of this if-chain matters, due to the chain of inheritance.
                    // Ie., don't ask a Habit if it's just a Task.
                    if (task is TaskManagement.Models.Habit)
                    {
                        // new() lets us cast to a subtype
                        newTask = new TaskManagement.Models.Habit("dummy desc", DateTime.Now, TaskManagement.Models.Frequency.Daily, 0);
                        newTaskExpectedTypeID = 2;
                    }
                    else if (task is TaskManagement.Models.RepeatingTask)
                    {
                        newTask = new TaskManagement.Models.RepeatingTask("dummy desc", DateTime.Now, TaskManagement.Models.Frequency.Daily);
                        newTaskExpectedTypeID = 1;
                    }
                    else
                    {
                        newTask = new("dummy desc");
                        newTaskExpectedTypeID = 0;
                    }

                    // Now we can test the type identifier...
                    int loadedTaskTypeID = SaveUtils.LoadAndPrintInt(reader);
                    Assert.AreEqual(newTaskExpectedTypeID, loadedTaskTypeID);

                    // ...and the loaded data. Polymorphic load here.
                    newTask.LoadFrom(reader);
                    Assert.AreEqual(task.ToString(), newTask.ToString());
                }
            }
        }

        /// <summary>
        /// Test binary save and load of TaskLists.
        /// </summary>
        [TestMethod]
        public void TestBinarySaveAndLoadTaskLists()
        {
            using (var stream = File.Open(TestFile.Path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    AT4TasksListA.SaveTo(writer);
                    AT4TasksListB.SaveTo(writer);
                }
            }

            using (var stream = File.Open(TestFolder.Path + "\\" + TestBinarySaveFilename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    // We don't have subtypes and a TaskList save manages
                    // its own count, so there's nothing complex here.
                    TaskManagement.Models.TaskList newTasksListA = new("dummy desc");
                    newTasksListA.LoadFrom(reader);
                    Assert.AreEqual(AT4TasksListA.ToString(), newTasksListA.ToString());

                    TaskManagement.Models.TaskList newTasksListB = new("dummy desc");
                    newTasksListB.LoadFrom(reader);
                    Assert.AreEqual(AT4TasksListB.ToString(), newTasksListB.ToString());
                }
            }
        }

        /// <summary>
        /// Test binary save and load of a TaskCollection.
        /// </summary>
        [TestMethod]
        public async System.Threading.Tasks.Task TestBinarySaveAndLoadTaskCollection()
        {
            String testBinarySaveFilename = "UnitTestSaveFile.bin";

            // TaskCollections take care of their own binary reader and
            // writer so testing them is quite mindless.
            await AT4TasksCollection.Save(testBinarySaveFilename);

            TaskCollection newTasksCollection = new();
            newTasksCollection.Load(testBinarySaveFilename);

            Assert.AreEqual(AT4TasksCollection.ToString(), newTasksCollection.ToString());
        }

        #endregion


        #region Assessment 5 Tests - Delegates

        //////////////////////////////////////////////////////////////////
        // Ctrl+M,L to un/fold. Unfold Setup() & Assessment 5 members.  //
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Test sorting TaskCollections by name.
        /// </summary>
        [TestMethod]
        public void TestSortCollectionTasksByName()
        {
            List<Task> sortedNoLists = NoListsCollection.GetTasksSortedByName();
            Assert.IsEmpty<Task>(sortedNoLists);

            List<Task> sortedNoTasks = NoTasksButListsCollection.GetTasksSortedByName();
            Assert.IsEmpty<Task>(sortedNoTasks);

            List<Task> sortedSomeTasks = TasksInSomeListsCollection.GetTasksSortedByName();
            Assert.AreEqual(10, sortedSomeTasks.Count);
            Assert.AreEqual(TaskForSorting1, sortedSomeTasks[0]);
            Assert.AreEqual(TaskForSorting2, sortedSomeTasks[1]);
            Assert.AreEqual(TaskForSorting3, sortedSomeTasks[2]);
            Assert.AreEqual(TaskForSorting4, sortedSomeTasks[3]);
            Assert.AreEqual(TaskForSorting5, sortedSomeTasks[4]);
            Assert.AreEqual(TaskForSorting8, sortedSomeTasks[5]);
            Assert.AreEqual(TaskForSorting9, sortedSomeTasks[6]);
            Assert.AreEqual(TaskForSorting10, sortedSomeTasks[7]);
            Assert.AreEqual(TaskForSorting11, sortedSomeTasks[8]);
            Assert.AreEqual(TaskForSorting12, sortedSomeTasks[9]);

            // We expected the new items to appeared inserted into the sequence sorted by name
            List<Task> sortedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetTasksSortedByName();
            Assert.AreEqual(14, sortedAllTasks.Count);
            Assert.AreEqual(TaskForSorting1, sortedAllTasks[0]);
            Assert.AreEqual(TaskForSorting2, sortedAllTasks[1]);
            Assert.AreEqual(TaskForSorting3, sortedAllTasks[2]);
            Assert.AreEqual(TaskForSorting4, sortedAllTasks[3]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[4]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[5]);
            Assert.AreEqual(TaskForSorting6, sortedAllTasks[6]);
            Assert.AreEqual(TaskForSorting7, sortedAllTasks[7]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[8]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[9]);
            Assert.AreEqual(TaskForSorting9, sortedAllTasks[10]);
            Assert.AreEqual(TaskForSorting10, sortedAllTasks[11]);
            Assert.AreEqual(TaskForSorting11, sortedAllTasks[12]);
            Assert.AreEqual(TaskForSorting12, sortedAllTasks[13]);
        }

        /// <summary>
        /// Test sorting TaskCollections by DueDate.
        /// </summary>
        [TestMethod]
        public void TestSortCollectionTasksByDate()
        {
            List<Task> sortedNoLists = NoListsCollection.GetTasksSortedByDate();
            Assert.IsEmpty<Task>(sortedNoLists);

            List<Task> sortedNoTasks = NoTasksButListsCollection.GetTasksSortedByDate();
            Assert.IsEmpty<Task>(sortedNoTasks);

            List<Task> sortedSomeTasks = TasksInSomeListsCollection.GetTasksSortedByDate();
            Assert.AreEqual(10, sortedSomeTasks.Count);
            Assert.AreEqual(TaskForSorting1, sortedSomeTasks[0]);
            Assert.AreEqual(TaskForSorting9, sortedSomeTasks[1]);
            Assert.AreEqual(TaskForSorting11, sortedSomeTasks[2]);
            // Multiple things could be due at the same time.
            // The TaskCollection is not expected to sort them any further.
            Assert.IsTrue(
                (sortedSomeTasks[3] == TaskForSorting2 && sortedSomeTasks[4] == TaskForSorting3) ||
                (sortedSomeTasks[3] == TaskForSorting3 && sortedSomeTasks[4] == TaskForSorting2));
            Assert.AreEqual(TaskForSorting4, sortedSomeTasks[5]);
            Assert.AreEqual(TaskForSorting5, sortedSomeTasks[6]);
            Assert.AreEqual(TaskForSorting10, sortedSomeTasks[7]);
            Assert.AreEqual(TaskForSorting12, sortedSomeTasks[8]);
            Assert.AreEqual(TaskForSorting8, sortedSomeTasks[9]);

            // We expected new ordering due to the new TaskList's Tasks
            List<Task> sortedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetTasksSortedByDate();
            Assert.AreEqual(14, sortedAllTasks.Count);
            Assert.AreEqual(TaskForSorting1, sortedAllTasks[0]);
            Assert.AreEqual(TaskForSorting7, sortedAllTasks[1]);
            Assert.AreEqual(TaskForSorting9, sortedAllTasks[2]);
            Assert.AreEqual(TaskForSorting11, sortedAllTasks[3]);
            Assert.IsTrue(
                (sortedAllTasks[4] == TaskForSorting2 && sortedAllTasks[5] == TaskForSorting3) ||
                (sortedAllTasks[4] == TaskForSorting3 && sortedAllTasks[5] == TaskForSorting2));
            Assert.AreEqual(TaskForSorting4, sortedAllTasks[6]);
            Assert.AreEqual(TaskForSorting6, sortedAllTasks[7]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[8]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[9]);
            Assert.AreEqual(TaskForSorting10, sortedAllTasks[10]);
            Assert.AreEqual(TaskForSorting12, sortedAllTasks[11]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[12]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[13]);
        }

        /// <summary>
        /// Test sorting TaskCollections by CreationDate.
        /// </summary>
        [TestMethod]
        public void TestSortCollectionTasksByCreationDate()
        {
            List<Task> sortedNoLists = NoListsCollection.GetTasksSortedByCreationDate();
            Assert.IsEmpty<Task>(sortedNoLists);

            List<Task> sortedNoTasks = NoTasksButListsCollection.GetTasksSortedByCreationDate();
            Assert.IsEmpty<Task>(sortedNoTasks);

            List<Task> sortedSomeTasks = TasksInSomeListsCollection.GetTasksSortedByCreationDate();
            Assert.AreEqual(10, sortedSomeTasks.Count);
            Assert.AreEqual(TaskForSorting1, sortedSomeTasks[0]);
            Assert.AreEqual(TaskForSorting2, sortedSomeTasks[1]);
            Assert.AreEqual(TaskForSorting3, sortedSomeTasks[2]);
            Assert.AreEqual(TaskForSorting4, sortedSomeTasks[3]);
            Assert.AreEqual(TaskForSorting5, sortedSomeTasks[4]);
            Assert.AreEqual(TaskForSorting8, sortedSomeTasks[5]);
            Assert.AreEqual(TaskForSorting9, sortedSomeTasks[6]);
            Assert.AreEqual(TaskForSorting10, sortedSomeTasks[7]);
            Assert.AreEqual(TaskForSorting11, sortedSomeTasks[8]);
            Assert.AreEqual(TaskForSorting12, sortedSomeTasks[9]);

            // We expected the new items to appeared inserted into the sequence sorted by creation
            // date of the Tasks themselves, not the TaskLists adding them to the TaskCollection,
            // or the dates of the Tasks' additions to those TaskLists.
            List<Task> sortedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetTasksSortedByCreationDate();
            Assert.AreEqual(14, sortedAllTasks.Count);
            Assert.AreEqual(TaskForSorting1, sortedAllTasks[0]);
            Assert.AreEqual(TaskForSorting2, sortedAllTasks[1]);
            Assert.AreEqual(TaskForSorting3, sortedAllTasks[2]);
            Assert.AreEqual(TaskForSorting4, sortedAllTasks[3]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[4]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[5]);
            Assert.AreEqual(TaskForSorting6, sortedAllTasks[6]);
            Assert.AreEqual(TaskForSorting7, sortedAllTasks[7]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[8]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[9]);
            Assert.AreEqual(TaskForSorting9, sortedAllTasks[10]);
            Assert.AreEqual(TaskForSorting10, sortedAllTasks[11]);
            Assert.AreEqual(TaskForSorting11, sortedAllTasks[12]);
            Assert.AreEqual(TaskForSorting12, sortedAllTasks[13]);
        }

        /// <summary>
        /// Test sorting TaskCollections by Priority.
        /// </summary>
        [TestMethod]
        public void TestSortCollectionTasksByPriority()
        {
            List<Task> sortedNoLists = NoListsCollection.GetTasksSortedByPriority();
            Assert.IsEmpty<Task>(sortedNoLists);

            List<Task> sortedNoTasks = NoTasksButListsCollection.GetTasksSortedByPriority();
            Assert.IsEmpty<Task>(sortedNoTasks);

            List<Task> sortedSomeTasks = TasksInSomeListsCollection.GetTasksSortedByPriority();
            Assert.AreEqual(10, sortedSomeTasks.Count);
            Assert.AreEqual(TaskForSorting5, sortedSomeTasks[0]);
            Assert.AreEqual(TaskForSorting8, sortedSomeTasks[1]);
            Assert.AreEqual(TaskForSorting4, sortedSomeTasks[2]);
            Assert.AreEqual(TaskForSorting9, sortedSomeTasks[3]);
            Assert.AreEqual(TaskForSorting3, sortedSomeTasks[4]);
            Assert.AreEqual(TaskForSorting10, sortedSomeTasks[5]);
            Assert.AreEqual(TaskForSorting2, sortedSomeTasks[6]);
            Assert.AreEqual(TaskForSorting11, sortedSomeTasks[7]);
            Assert.AreEqual(TaskForSorting1, sortedSomeTasks[8]);
            Assert.AreEqual(TaskForSorting12, sortedSomeTasks[9]);

            // We expected the new items to appeared inserted into the sequence sorted by creation
            // date of the Tasks themselves, not the TaskLists adding them to the TaskCollection,
            // or the dates of the Tasks' additions to those TaskLists.
            List<Task> sortedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetTasksSortedByPriority();
            Assert.AreEqual(14, sortedAllTasks.Count);
            Assert.AreEqual(TaskForSorting6, sortedAllTasks[0]);
            Assert.AreEqual(TaskForSorting7, sortedAllTasks[1]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[2]);
            Assert.AreEqual(TaskForSorting5, sortedAllTasks[3]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[4]);
            Assert.AreEqual(TaskForSorting8, sortedAllTasks[5]);
            Assert.AreEqual(TaskForSorting4, sortedAllTasks[6]);
            Assert.AreEqual(TaskForSorting9, sortedAllTasks[7]);
            Assert.AreEqual(TaskForSorting3, sortedAllTasks[8]);
            Assert.AreEqual(TaskForSorting10, sortedAllTasks[9]);
            Assert.AreEqual(TaskForSorting2, sortedAllTasks[10]);
            Assert.AreEqual(TaskForSorting11, sortedAllTasks[11]);
            Assert.AreEqual(TaskForSorting1, sortedAllTasks[12]);
            Assert.AreEqual(TaskForSorting12, sortedAllTasks[13]);
        }

        /// <summary>
        /// Test getting all Habits only from a TaskCollection.
        /// </summary>
        [TestMethod]
        public void TestGetHabits()
        {
            List<Task> searchedNoLists = NoListsCollection.GetHabits();
            Assert.IsEmpty<Task>(searchedNoLists);

            List<Task> searchedNoTasks = NoTasksButListsCollection.GetHabits();
            Assert.IsEmpty<Task>(searchedNoTasks);

            List<Task> searchedSomeTasks = TasksInSomeListsCollection.GetHabits();
            Assert.AreEqual(2, searchedSomeTasks.Count);
            Assert.Contains(TaskForSorting5, searchedSomeTasks);
            Assert.Contains(TaskForSorting8, searchedSomeTasks);

            List<Task> searchedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetHabits();
            Assert.AreEqual(6, searchedAllTasks.Count);
            Assert.Contains(TaskForSorting5, searchedAllTasks);
            Assert.Contains(TaskForSorting6, searchedAllTasks);
            Assert.Contains(TaskForSorting7, searchedAllTasks);
            Assert.Contains(TaskForSorting8, searchedAllTasks);
            // LINQ jumping in to make life easy for counting expected duplicates
            Assert.AreEqual(2, searchedAllTasks.Count(task => task == TaskForSorting5));
            Assert.AreEqual(2, searchedAllTasks.Count(task => task == TaskForSorting8));
        }

        /// <summary>
        /// Test getting all RepeatingTasks only from a TaskCollection.
        /// </summary>
        [TestMethod]
        public void TestGetRepeatingTasks()
        {
            List<Task> searchedNoLists = NoListsCollection.GetRepeatingTasks();
            Assert.IsEmpty<Task>(searchedNoLists);

            List<Task> searchedNoTasks = NoTasksButListsCollection.GetRepeatingTasks();
            Assert.IsEmpty<Task>(searchedNoTasks);

            List<Task> searchedSomeTasks = TasksInSomeListsCollection.GetRepeatingTasks();
            Assert.AreEqual(6, searchedSomeTasks.Count);
            // Every Habit is still a RepeatingTask
            Assert.Contains(TaskForSorting5, searchedSomeTasks);
            Assert.Contains(TaskForSorting8, searchedSomeTasks);
            Assert.Contains(TaskForSorting9, searchedSomeTasks);
            Assert.Contains(TaskForSorting10, searchedSomeTasks);
            Assert.Contains(TaskForSorting11, searchedSomeTasks);
            Assert.Contains(TaskForSorting12, searchedSomeTasks);

            List<Task> searchedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetRepeatingTasks();
            Assert.AreEqual(10, searchedAllTasks.Count);
            Assert.Contains(TaskForSorting5, searchedAllTasks);
            Assert.Contains(TaskForSorting6, searchedAllTasks);
            Assert.Contains(TaskForSorting7, searchedAllTasks);
            Assert.Contains(TaskForSorting8, searchedAllTasks);
            Assert.Contains(TaskForSorting9, searchedAllTasks);
            Assert.Contains(TaskForSorting10, searchedAllTasks);
            Assert.Contains(TaskForSorting11, searchedAllTasks);
            Assert.Contains(TaskForSorting12, searchedAllTasks);
            Assert.AreEqual(2, searchedAllTasks.Count(task => task == TaskForSorting5));
            Assert.AreEqual(2, searchedAllTasks.Count(task => task == TaskForSorting8));
        }

        /// <summary>
        /// Test getting all Tasks due today only from a TaskCollection.
        /// </summary>
        [TestMethod]
        public void TestGetDueTasks()
        {
            List<Task> searchedNoLists = NoListsCollection.GetDueTasks();
            Assert.AreEqual("\n ------------------------ \n", NoListsCollection.ToString());

            List<Task> searchedNoTasks = NoTasksButListsCollection.GetDueTasks();
            Assert.AreEqual("\n ------------------------ \n", NoTasksButListsCollection.ToString());

            List<Task> searchedSomeTasks = TasksInSomeListsCollection.GetDueTasks();
            Assert.AreEqual(2, searchedSomeTasks.Count);
            Assert.Contains(TaskForSorting2, searchedSomeTasks);
            Assert.Contains(TaskForSorting3, searchedSomeTasks);

            List<Task> searchedAllTasks = AllListsHaveTasksWithDuplicatesCollection.GetDueTasks();
            Assert.AreEqual(2, searchedAllTasks.Count);
            Assert.Contains(TaskForSorting2, searchedAllTasks);
            Assert.Contains(TaskForSorting3, searchedAllTasks);
        }

        /// <summary>
        /// Test getting all Tasks with a description from a TaskCollection.
        /// </summary>
        [TestMethod]
        public void TestGetTaskByDescription()
        {
            // Empty strings
            List<Task> searchedNoListsForEmptyString = NoListsCollection.GetTasksWithDescription("");
            Assert.IsEmpty<Task>(searchedNoListsForEmptyString);

            List<Task> searchedNoTasksForEmptyString = NoTasksButListsCollection.GetTasksWithDescription("");
            Assert.IsEmpty<Task>(searchedNoTasksForEmptyString);

            List<Task> searchedSomeTasksForEmptyString = TasksInSomeListsCollection.GetTasksWithDescription("");
            Assert.IsEmpty<Task>(searchedSomeTasksForEmptyString);

            List<Task> searchedAllTasksForEmptyString = AllListsHaveTasksWithDuplicatesCollection.GetTasksWithDescription("");
            Assert.IsEmpty<Task>(searchedAllTasksForEmptyString);

            // Non-existing string
            string badString = "I'm that bad string, Make your mama sad string, Make your girlfriend mad string, Might seduce your dad string. I'm the bad string. Duh.";
            List<Task> searchedNoListsForSillyString = NoListsCollection.GetTasksWithDescription(badString);
            Assert.IsEmpty<Task>(searchedNoListsForSillyString);

            List<Task> searchedNoTasksForSillyString = NoTasksButListsCollection.GetTasksWithDescription(badString);
            Assert.IsEmpty<Task>(searchedNoTasksForSillyString);

            List<Task> searchedSomeTasksForSillyString = TasksInSomeListsCollection.GetTasksWithDescription(badString);
            Assert.IsEmpty<Task>(searchedSomeTasksForSillyString);

            List<Task> searchedAllTasksForSillyString = AllListsHaveTasksWithDuplicatesCollection.GetTasksWithDescription(badString);
            Assert.IsEmpty<Task>(searchedAllTasksForSillyString);

            // Existing strings
            string goodString = "Heat the grater";  // Matches TaskForSorting8
            List<Task> searchedNoListsForExistingString = NoListsCollection.GetTasksWithDescription(goodString);
            Assert.IsEmpty<Task>(searchedNoListsForExistingString);

            List<Task> searchedNoTasksForExistingString = NoTasksButListsCollection.GetTasksWithDescription(goodString);
            Assert.IsEmpty<Task>(searchedNoTasksForExistingString);

            List<Task> searchedSomeTasksForExistingString = TasksInSomeListsCollection.GetTasksWithDescription(goodString);
            Assert.AreEqual(1, searchedSomeTasksForExistingString.Count);
            Assert.AreEqual(TaskForSorting8, searchedSomeTasksForExistingString[0]);

            List<Task> searchedAllTasksForExistingString = AllListsHaveTasksWithDuplicatesCollection.GetTasksWithDescription(goodString);
            Assert.AreEqual(2, searchedAllTasksForExistingString.Count);
            Assert.AreEqual(TaskForSorting8, searchedAllTasksForExistingString[0]);
            Assert.AreEqual(TaskForSorting8, searchedAllTasksForExistingString[1]);
        }

        #endregion


        #region Assessment 7 Tests - Forgiving Format

        /// <summary>
        /// Test that natural dates specified for this week are producing the correct dates.
        /// The majority of the remaining tests test for format and DayOfWeek.
        /// </summary>
        [TestMethod]
        public void TestThisWeekWeekdayCounting()
        {
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Sunday"));
                    break;

                case DayOfWeek.Tuesday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Sunday"));
                    break;

                case DayOfWeek.Wednesday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Sunday"));
                    break;

                case DayOfWeek.Thursday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Sunday"));
                    break;

                case DayOfWeek.Friday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Sunday"));
                    break;

                case DayOfWeek.Saturday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Sunday"));
                    break;

                case DayOfWeek.Sunday:
                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Sunday"));

                    Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("This Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("This Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("This Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("This Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("This Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("This Sunday"));
                    break;
            }
        }

        /// <summary>
        /// Test that natural dates specified for next week are producing the correct dates.
        /// The majority of the remaining tests test for format and DayOfWeek.
        /// </summary>
        [TestMethod]
        public void TestNextWeekWeekdayCounting()
        {
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(9), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(10), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(11), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(12), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(13), StringParse.ParseNaturalDate("Next Sunday"));
                    break;

                case DayOfWeek.Tuesday:
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(9), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(10), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(11), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(12), StringParse.ParseNaturalDate("Next Sunday"));
                    break;

                case DayOfWeek.Wednesday:
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(9), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(10), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(11), StringParse.ParseNaturalDate("Next Sunday"));
                    break;

                case DayOfWeek.Thursday:
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(9), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(10), StringParse.ParseNaturalDate("Next Sunday"));
                    break;

                case DayOfWeek.Friday:
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(9), StringParse.ParseNaturalDate("Next Sunday"));
                    break;

                case DayOfWeek.Saturday:
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Sunday"));
                    break;

                case DayOfWeek.Sunday:  // Weeks are assumed to be from Monday to Sunday
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Today"));
                    Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
                    Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Next Monday"));
                    Assert.AreEqual(DateTime.Today.AddDays(2), StringParse.ParseNaturalDate("Next Tuesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(3), StringParse.ParseNaturalDate("Next Wednesday"));
                    Assert.AreEqual(DateTime.Today.AddDays(4), StringParse.ParseNaturalDate("Next Thursday"));
                    Assert.AreEqual(DateTime.Today.AddDays(5), StringParse.ParseNaturalDate("Next Friday"));
                    Assert.AreEqual(DateTime.Today.AddDays(6), StringParse.ParseNaturalDate("Next Saturday"));
                    Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next Sunday"));
                    break;
            }
        }

        [TestMethod]
        public void TestValidOneWordTodayStrings()
        {
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Today"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("TODAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("today"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("TodAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("toady"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("Todya"));
        }

        [TestMethod]
        public void TestValidOneWordTomorrowStrings()
        {
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("Tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("TOMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("TOMMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("tomoro"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("torMOrrow"));
        }

        [TestMethod]
        public void TestValidOneWordDayStrings()
        {
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("Monday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("MONDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("monday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("MOndAY").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("Tuesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("TUESDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("tuesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("tUeSDay").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("Wednesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("WEDNESDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("wednesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("wEdNesDaY").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("Thursday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("THURSDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("thursday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("ThUrSdAy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("Friday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("FRIDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("friday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("frIdAy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("Saturday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("SATURDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("saturday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("SaTuRDay").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("Sunday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("SUNDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("sunday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("SunDAy").DayOfWeek);
        }

        [TestMethod]
        public void TestValidOneWordMisspelledDayStrings()
        {
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("mOnDya").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("MonAdY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("moNDy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("tEuSday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("tUesdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("tuesDAy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("wEnSDay").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("weDNesDaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("wEnDeSday").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("thuRsdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("ThUrSdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("thursDAe").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("fIrDaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("frIDa").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("FrIdAy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("saTerDay").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("SaTuRdaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("satURdae").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("suNdYa").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("sUnDaE").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("SuNdAy").DayOfWeek);
        }

        [TestMethod]
        public void TestInvalidOneWordDayStrings()
        {
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("yadot"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("tmrrw"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("yesterday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("moondays"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("choosedays"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("weddingday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("thearlsday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Fry-upday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Sitddownday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("sunnight"));
        }

        [TestMethod]
        public void TestTwoWordTodayStringsWithValidPreposition()
        {
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("AT today"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("oN TODAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("By todAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("in todya"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("FOR Today"));
        }

        [TestMethod]
        public void TestTwoWordTomorrowStringsWithValidPreposition()
        {
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("at tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("ON TOMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("bY toMORRow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("In tommorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("fOr Tomorrow"));
        }

        [TestMethod]
        public void TestTwoWordDayStringsWithValidPreposition()
        {
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("at Monday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("aT moNDya").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("on tuesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("ON TeUsDay").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("By wedNESday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("by wEnDeSday").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("iN THURSDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("iN ThUrSday").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("fOr fridAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("FOr FiRdAy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("on saturday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("on saTerDay").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("BY Sunday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("by suNdYa").DayOfWeek);
        }

        [TestMethod]
        public void TestTwoWordTodayStringsWithWeekSpecifier()
        {
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("THIS today"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("This TODAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("this todAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("THis todya"));

            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("NEXT today"));
            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("next TODAY"));
            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("Next todAY"));
            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("NeXT todya"));
        }

        [TestMethod]
        public void TestTwoWordTomorrowStringsWithWeekSpecifier()
        {
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("THIS tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("This TOMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("this Tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("THis tomORRow"));

            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("NEXT tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("next TOMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("Next Tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("NeXT tOMORroW"));
        }

        [TestMethod]
        public void TestTwoWordDayStringsWithWeekSpecifier()
        {
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("THIS Monday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("This tuesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("this WEDnesDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("THis Thusday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("NEXT Thursday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("next FRIDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("Next saturday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("NeXT SUNday").DayOfWeek);
        }

        [TestMethod]
        public void TestTwoWordDayStringsWithInvalidFirstWord()
        {
            try
            {
                StringParse.ParseNaturalDate("a today");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("n tomorrow");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("previous Monday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("thsi Tuesday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("that Wednesday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("nxet Thursday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("tish Friday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("t Saturday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalDate("through Sunday");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural date format.", ex.Message);
            }
        }

        [TestMethod]
        public void TestValidThreeWordTodayStrings()
        {
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("AT This TODAY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("FOR this ToDaY"));
            Assert.AreEqual(DateTime.Today, StringParse.ParseNaturalDate("in THIS toDyA"));
            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("on NEXT today"));
            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("By Next TODAY"));
            Assert.AreEqual(DateTime.Today.AddDays(7), StringParse.ParseNaturalDate("FOR next ToAdY"));
        }

        [TestMethod]
        public void TestValidThreeWordTomorrowStrings()
        {
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("ON This TOMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("BY this ToMoRrOw"));
            Assert.AreEqual(DateTime.Today.AddDays(1), StringParse.ParseNaturalDate("at THIS tOmMoRoW"));
            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("In Next TOMORROW"));
            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("for NEXT tomorrow"));
            Assert.AreEqual(DateTime.Today.AddDays(8), StringParse.ParseNaturalDate("BY next ToMoRoW"));
        }

        [TestMethod]
        public void TestValidThreeWordDayStrings()
        {
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("AT This MONDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("on NEXT monday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("FOR this MoNdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("By Next MONDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("in THIS mOnDyA").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, StringParse.ParseNaturalDate("FOR next MonAdY").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("ON This TUESDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("for NEXT tuesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("BY this TueSdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("In Next TUESDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("at THIS teUsDAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Tuesday, StringParse.ParseNaturalDate("BY next TuEsDyA").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("FOR This WEDNESDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("by NEXT wednesday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("IN this WedNESday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("At Next wEdNesDaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("ON THIS wEnSdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Wednesday, StringParse.ParseNaturalDate("IN next WedNeSdY").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("BY This THURSDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("in NEXT thursday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("AT this ThUrSdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("On Next THURSDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("FOR THIS thuRdSaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Thursday, StringParse.ParseNaturalDate("at NEXT ThRuSdAy").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("IN This FRIDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("at NEXT friday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("ON this FrIdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("For Next FRIDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("BY THIS FiRdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, StringParse.ParseNaturalDate("on NEXT FrIdYa").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("AT This SATURDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("on NEXT saturday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("BY this SaTErDaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("In Next SATURDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("FOR THIS sAtErDaY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, StringParse.ParseNaturalDate("AT next SaTuRdY").DayOfWeek);

            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("ON This SUNDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("for NEXT sunday").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("IN this SuNdAy").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("At Next SUNDAY").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("BY THIS SuNdYa").DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, StringParse.ParseNaturalDate("ON next SuNdY").DayOfWeek);
        }

        [TestMethod]
        public void TestUnorderedThreeWordDayStrings()
        {
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Monday this by"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Next Tuesday on"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("By Wednesday this"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("This Thursday in"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Friday next at"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("On Saturday this"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Sunday by next"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Next today by"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalDate("Tomorrow this in"));
        }

        [TestMethod]
        public void TestLongTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(16) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("At half past 4 pm"));
            Assert.AreEqual(TimeSpan.FromHours(11) + TimeSpan.FromMinutes(45), StringParse.ParseNaturalTime("Before a quarter to 12"));
            Assert.AreEqual(TimeSpan.FromHours(19) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("For seven thirty o'clock in the evening"));
            Assert.AreEqual(TimeSpan.FromHours(4) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("By a half past 4 in the morning"));
            Assert.AreEqual(TimeSpan.FromHours(07), StringParse.ParseNaturalTime("On seven o'clock am"));
            Assert.AreEqual(TimeSpan.FromHours(21) + TimeSpan.FromMinutes(13), StringParse.ParseNaturalTime("At 9:13 pm"));
            Assert.AreEqual(TimeSpan.FromHours(20) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("Before Twenty Thirty"));
        }

        [TestMethod]
        public void TestShortTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("1"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("One"));
            Assert.AreEqual(TimeSpan.FromHours(1), StringParse.ParseNaturalTime("01"));
            Assert.AreEqual(TimeSpan.FromHours(1), StringParse.ParseNaturalTime("1 am"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("1 pm"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("13"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("6"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("06"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("Six"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("6 am"));
            Assert.AreEqual(TimeSpan.FromHours(18), StringParse.ParseNaturalTime("6 pm"));
            Assert.AreEqual(TimeSpan.FromHours(18), StringParse.ParseNaturalTime("18"));
        }

        [TestMethod]
        public void TestValidNumberedHourOnlyTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(0), StringParse.ParseNaturalTime("0"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("1"));
            Assert.AreEqual(TimeSpan.FromHours(15), StringParse.ParseNaturalTime("3"));
            Assert.AreEqual(TimeSpan.FromHours(5), StringParse.ParseNaturalTime("5"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("6"));
            Assert.AreEqual(TimeSpan.FromHours(12), StringParse.ParseNaturalTime("12"));
            Assert.AreEqual(TimeSpan.FromHours(14), StringParse.ParseNaturalTime("14"));
            Assert.AreEqual(TimeSpan.FromHours(21), StringParse.ParseNaturalTime("21"));
        }

        [TestMethod]
        public void TestValidNamedHourOnlyTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(0), StringParse.ParseNaturalTime("Zero"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("one"));
            Assert.AreEqual(TimeSpan.FromHours(15), StringParse.ParseNaturalTime("There"));
            Assert.AreEqual(TimeSpan.FromHours(5), StringParse.ParseNaturalTime("Fiv"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("Sic"));
            Assert.AreEqual(TimeSpan.FromHours(12), StringParse.ParseNaturalTime("TWELVE"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("THirtEEN"));
            Assert.AreEqual(TimeSpan.FromHours(20), StringParse.ParseNaturalTime("twenty"));
        }

        [TestMethod]
        public void TestInvalidHourOnlyTimeStrings()
        {
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("24"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("000"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("017"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("twenty-one"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("bigfoot"));
        }

        [TestMethod]
        public void TestValidPrepositionTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(0), StringParse.ParseNaturalTime("at 0"));
            Assert.AreEqual(TimeSpan.FromHours(14), StringParse.ParseNaturalTime("BY 2"));
            Assert.AreEqual(TimeSpan.FromHours(16), StringParse.ParseNaturalTime("For 4"));
            Assert.AreEqual(TimeSpan.FromHours(7), StringParse.ParseNaturalTime("oN 7"));
            Assert.AreEqual(TimeSpan.FromHours(8), StringParse.ParseNaturalTime("beFOre 8"));
            Assert.AreEqual(TimeSpan.FromHours(12), StringParse.ParseNaturalTime("A 12"));
            Assert.AreEqual(TimeSpan.FromHours(14), StringParse.ParseNaturalTime("a 14"));
            Assert.AreEqual(TimeSpan.FromHours(21), StringParse.ParseNaturalTime("At 21"));
            Assert.AreEqual(TimeSpan.FromHours(0), StringParse.ParseNaturalTime("bY zero"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("on one"));
            Assert.AreEqual(TimeSpan.FromHours(15), StringParse.ParseNaturalTime("a three"));
            Assert.AreEqual(TimeSpan.FromHours(5), StringParse.ParseNaturalTime("on five"));
            Assert.AreEqual(TimeSpan.FromHours(6), StringParse.ParseNaturalTime("before six"));
            Assert.AreEqual(TimeSpan.FromHours(12), StringParse.ParseNaturalTime("by twelve"));
            Assert.AreEqual(TimeSpan.FromHours(13), StringParse.ParseNaturalTime("BY thirteen"));
            Assert.AreEqual(TimeSpan.FromHours(20), StringParse.ParseNaturalTime("On twenty"));
        }

        [TestMethod]
        public void TestValidClockSegmentTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(23) + TimeSpan.FromMinutes(45), StringParse.ParseNaturalTime("a quarter to 0"));
            Assert.AreEqual(TimeSpan.FromHours(13) + TimeSpan.FromMinutes(45), StringParse.ParseNaturalTime("Quarter To 2"));
            Assert.AreEqual(TimeSpan.FromHours(15) + TimeSpan.FromMinutes(45), StringParse.ParseNaturalTime("a qUATER tO 4"));
            Assert.AreEqual(TimeSpan.FromHours(6) + TimeSpan.FromMinutes(45), StringParse.ParseNaturalTime("QUARFTER TO 7"));
            Assert.AreEqual(TimeSpan.FromHours(8) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("quarter past 8"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("a qarTER PAST 12"));
            Assert.AreEqual(TimeSpan.FromHours(14) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("QUORTER Past 14"));
            Assert.AreEqual(TimeSpan.FromHours(21) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("a Quarterr pASt 21"));
            Assert.AreEqual(TimeSpan.FromHours(23) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("half to zero"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("a Half TO one"));
            Assert.AreEqual(TimeSpan.FromHours(14) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("HALF tO three"));
            Assert.AreEqual(TimeSpan.FromHours(4) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("a hALf To five"));
            Assert.AreEqual(TimeSpan.FromHours(6) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("half past six"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("a HALF Past twelve"));
            Assert.AreEqual(TimeSpan.FromHours(13) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("Half PAST thirteen"));
            Assert.AreEqual(TimeSpan.FromHours(20) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("a haLF PAst twenty"));
        }

        [TestMethod]
        public void TestInvalidClockSegmentTimeStrings()
        {
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("a quarter 12"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("half seven"));
        }

        [TestMethod]
        public void TestValidHoursAndMinutesTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(0) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("zero thirty"));
            Assert.AreEqual(TimeSpan.FromHours(13) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("one:THIRty"));
            Assert.AreEqual(TimeSpan.FromHours(15) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("three 30"));
            Assert.AreEqual(TimeSpan.FromHours(15) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("three.30"));
            Assert.AreEqual(TimeSpan.FromHours(5) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("five fiFTEEN"));
            Assert.AreEqual(TimeSpan.FromHours(6) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("six.fifteen"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("twelve 15"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("twelve:15"));
            Assert.AreEqual(TimeSpan.FromHours(13) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("13,30"));
            Assert.AreEqual(TimeSpan.FromHours(20) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("20:THIRTY"));
            Assert.AreEqual(TimeSpan.FromHours(9) + TimeSpan.FromMinutes(2), StringParse.ParseNaturalTime("9:02"));
            Assert.AreEqual(TimeSpan.FromHours(9) + TimeSpan.FromMinutes(22), StringParse.ParseNaturalTime("9 22"));
            Assert.AreEqual(TimeSpan.FromHours(14) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("14,fifteen"));
            Assert.AreEqual(TimeSpan.FromHours(14) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("14.15"));
        }

        [TestMethod]
        public void TestInvalidMinutesTimeStrings()
        {
            try
            {
                StringParse.ParseNaturalTime("One forty");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural time format: forty", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalTime("Seven 60");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural time format: 60", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalTime("6:412");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid natural time format: 412", ex.Message);
            }

            try
            {
                StringParse.ParseNaturalTime("8 -15");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid punctuation found: 8 -15", ex.Message);
            }
        }

        [TestMethod]
        public void TestValidSidesOfNoonTimeStrings()
        {
            Assert.AreEqual(TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0), StringParse.ParseNaturalTime("zero"));
            Assert.AreEqual(TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("one thirty in the morning"));
            Assert.AreEqual(TimeSpan.FromHours(15) + TimeSpan.FromMinutes(12), StringParse.ParseNaturalTime("three 12 AFternooN"));
            Assert.AreEqual(TimeSpan.FromHours(15) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("three fifteen oclock pm"));
            Assert.AreEqual(TimeSpan.FromHours(17) + TimeSpan.FromMinutes(0), StringParse.ParseNaturalTime("five in evening"));
            Assert.AreEqual(TimeSpan.FromHours(6) + TimeSpan.FromMinutes(0), StringParse.ParseNaturalTime("six at mornin"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(0), StringParse.ParseNaturalTime("twelve in the o'clock Aternoon"));
            Assert.AreEqual(TimeSpan.FromHours(12) + TimeSpan.FromMinutes(0), StringParse.ParseNaturalTime("twelve pm"));
            Assert.AreEqual(TimeSpan.FromHours(9) + TimeSpan.FromMinutes(30), StringParse.ParseNaturalTime("9:thirty in the MORING"));
            Assert.AreEqual(TimeSpan.FromHours(21) + TimeSpan.FromMinutes(22), StringParse.ParseNaturalTime("9 22 in the evning"));
            Assert.AreEqual(TimeSpan.FromHours(14) + TimeSpan.FromMinutes(15), StringParse.ParseNaturalTime("2.15 Pm"));
        }

        [TestMethod]
        public void TestInvalidSidesOfNoonTimeStrings()
        {
            // Self-contradicting time
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("14 am"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("08 pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("thirteen am"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("zero pm"));

            // Redundant am/pm
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("07 am"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("15 pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("fourteen pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("zero am"));

            // Self-contradicting elaborate time 
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("14:01 in the morning"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("half past 08 pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("zero o'clock pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("thirteen 20 am"));

            // Redundant elaborate time
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("07 thirty am"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("a quarter to 15 pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("fourteen fifteen pm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("zero 00 am"));

            // Bad am/pm
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("14 dm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("07 bm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("thirteen qm"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTime("zero pn"));
        }

        [TestMethod]
        public void TestNaturalTasksWithValidTimes()
        {
            Task newTask1 = StringParse.ParseNaturalTaskCreation("Buy a pony at 9");
            Task newTask2 = StringParse.ParseNaturalTaskCreation("At half past 4 pm  Tell John To Get A Dog");
            Task newTask3 = StringParse.ParseNaturalTaskCreation("Eat, Before 9:12 am, the pies");
            Task newTask4 = StringParse.ParseNaturalTaskCreation("Call Pope... Before a quarter to 12");
            Task newTask5 = StringParse.ParseNaturalTaskCreation("RESISTANCE IS For seven thirty o'clock in the evening FUTILE!");

            Assert.AreEqual("Buy a pony", newTask1.GetDescription());
            Assert.AreEqual(DateTime.Today + TimeSpan.FromHours(9), newTask1.DueDate);

            Assert.AreEqual("Tell John To Get A Dog", newTask2.GetDescription());
            Assert.AreEqual(DateTime.Today + TimeSpan.FromHours(16.5), newTask2.DueDate);

            Assert.AreEqual("Eat the pies", newTask3.GetDescription());
            Assert.AreEqual(DateTime.Today + TimeSpan.FromHours(9) + TimeSpan.FromMinutes(12), newTask3.DueDate);

            Assert.AreEqual("Call Pope", newTask4.GetDescription());
            Assert.AreEqual(DateTime.Today + TimeSpan.FromHours(11.75), newTask4.DueDate);

            Assert.AreEqual("RESISTANCE IS FUTILE", newTask5.GetDescription());
            Assert.AreEqual(DateTime.Today + TimeSpan.FromHours(19.5), newTask5.DueDate);
        }

        [TestMethod]
        public void TestNaturalTasksWithValidDates()
        {
            Task newTask1 = StringParse.ParseNaturalTaskCreation("Eat by MONDAY the pies");
            Task newTask2 = StringParse.ParseNaturalTaskCreation("Buy some books ON TUESDAY");
            Task newTask3 = StringParse.ParseNaturalTaskCreation("Remind myself to pay the bills AT SATURDAY");
            Task newTask4 = StringParse.ParseNaturalTaskCreation("next   tomorrow Buy kittens or something");
            Task newTask5 = StringParse.ParseNaturalTaskCreation("On this today... just chill");
            Task newTask6 = StringParse.ParseNaturalTaskCreation("Make a note for Friday to summon Gandalf");

            Assert.AreEqual("Eat the pies", newTask1.GetDescription());
            Assert.AreEqual(DayOfWeek.Monday, newTask1.DueDate.Value.DayOfWeek);

            Assert.AreEqual("Buy some books", newTask2.GetDescription());
            Assert.AreEqual(DayOfWeek.Tuesday, newTask2.DueDate.Value.DayOfWeek);

            Assert.AreEqual("Remind myself to pay the bills", newTask3.GetDescription());
            Assert.AreEqual(DayOfWeek.Saturday, newTask3.DueDate.Value.DayOfWeek);

            Assert.AreEqual("Buy kittens or something", newTask4.GetDescription());
            Assert.AreEqual(DateTime.Today.AddDays(8), newTask4.DueDate);

            Assert.AreEqual("just chill", newTask5.GetDescription());
            Assert.AreEqual(DateTime.Today, newTask5.DueDate);

            Assert.AreEqual("Make a note to summon Gandalf", newTask6.GetDescription());
            Assert.AreEqual(DayOfWeek.Friday, newTask6.DueDate.Value.DayOfWeek);
        }

        [TestMethod]
        public void TestNaturalTasksWithValidDatesAndTimes()
        {
            Task newTask1 = StringParse.ParseNaturalTaskCreation("Eat the food by MONDAY at 12 o'clock");
            Task newTask2 = StringParse.ParseNaturalTaskCreation("Go to town at 7:10 am on Tuesday");
            Task newTask3 = StringParse.ParseNaturalTaskCreation("On Wednesday, buy a coat, at 4 oclock am");
            Task newTask4 = StringParse.ParseNaturalTaskCreation("Before zero get outta town on Thursday");
            Task newTask5 = StringParse.ParseNaturalTaskCreation("For Friday by 6:05 make pancakes");
            Task newTask6 = StringParse.ParseNaturalTaskCreation("At twenty:17 on Satuday get jiggy with it");

            Assert.AreEqual("Eat the food", newTask1.GetDescription());
            Assert.AreEqual(DayOfWeek.Monday, newTask1.DueDate.Value.DayOfWeek);
            Assert.AreEqual(12, newTask1.DueDate.Value.Hour);
            Assert.AreEqual(0, newTask1.DueDate.Value.Minute);

            Assert.AreEqual("Go to town", newTask2.GetDescription());
            Assert.AreEqual(DayOfWeek.Tuesday, newTask2.DueDate.Value.DayOfWeek);
            Assert.AreEqual(7, newTask2.DueDate.Value.Hour);
            Assert.AreEqual(10, newTask2.DueDate.Value.Minute);

            Assert.AreEqual("buy a coat", newTask3.GetDescription());
            Assert.AreEqual(DayOfWeek.Wednesday, newTask3.DueDate.Value.DayOfWeek);
            Assert.AreEqual(4, newTask3.DueDate.Value.Hour);
            Assert.AreEqual(0, newTask3.DueDate.Value.Minute);

            Assert.AreEqual("get outta town", newTask4.GetDescription());
            Assert.AreEqual(DayOfWeek.Thursday, newTask4.DueDate.Value.DayOfWeek);
            Assert.AreEqual(0, newTask4.DueDate.Value.Hour);
            Assert.AreEqual(0, newTask4.DueDate.Value.Minute);

            Assert.AreEqual("make pancakes", newTask5.GetDescription());
            Assert.AreEqual(DayOfWeek.Friday, newTask5.DueDate.Value.DayOfWeek);
            Assert.AreEqual(6, newTask5.DueDate.Value.Hour);
            Assert.AreEqual(5, newTask5.DueDate.Value.Minute);

            Assert.AreEqual("get jiggy with it", newTask6.GetDescription());
            Assert.AreEqual(DayOfWeek.Saturday, newTask6.DueDate.Value.DayOfWeek);
            Assert.AreEqual(20, newTask6.DueDate.Value.Hour);
            Assert.AreEqual(17, newTask6.DueDate.Value.Minute);
        }

        [TestMethod]
        public void TestAssessmentExamples()
        {
            Task testTask = new("Call Rob");

            int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)DateTime.Today.DayOfWeek + 7) % 7;
            testTask.DueDate = DateTime.Today + TimeSpan.FromDays(daysUntilWednesday) + TimeSpan.FromHours(15);

            Task newTask1 = StringParse.ParseNaturalTaskCreation("Call Rob on Wednesday at three PM");
            Task newTask2 = StringParse.ParseNaturalTaskCreation("Call Rob at three PM on Wednesday");
            Task newTask3 = StringParse.ParseNaturalTaskCreation("Call Rob");
            Task newTask4 = StringParse.ParseNaturalTaskCreation("Call Rob, three PM, Wednesday");
            Task newTask5 = StringParse.ParseNaturalTaskCreation("Call Rob, Wednesday, three PM");
            Task newTask6 = StringParse.ParseNaturalTaskCreation("Call Rob three PM Wednesday");
            Task newTask7 = StringParse.ParseNaturalTaskCreation("Call Rob Wednesday three PM");

            Assert.AreEqual(testTask.GetDescription(), newTask1.GetDescription());
            Assert.AreEqual(testTask.DueDate, newTask1.DueDate);
            Assert.AreEqual(testTask.GetDescription(), newTask1.GetDescription());

            Assert.AreEqual(testTask.GetDescription(), newTask2.GetDescription());
            Assert.AreEqual(testTask.DueDate, newTask2.DueDate);
            Assert.AreEqual(testTask.GetDescription(), newTask2.GetDescription());

            Assert.AreEqual(testTask.GetDescription(), newTask3.GetDescription());

            Assert.AreEqual(testTask.GetDescription(), newTask4.GetDescription());
            Assert.AreEqual(testTask.DueDate, newTask4.DueDate);
            Assert.AreEqual(testTask.GetDescription(), newTask4.GetDescription());

            Assert.AreEqual(testTask.GetDescription(), newTask5.GetDescription());
            Assert.AreEqual(testTask.DueDate, newTask5.DueDate);
            Assert.AreEqual(testTask.GetDescription(), newTask5.GetDescription());

            Assert.AreEqual(testTask.GetDescription(), newTask6.GetDescription());
            Assert.AreEqual(testTask.DueDate, newTask6.DueDate);
            Assert.AreEqual(testTask.GetDescription(), newTask6.GetDescription());

            Assert.AreEqual(testTask.GetDescription(), newTask7.GetDescription());
            Assert.AreEqual(testTask.DueDate, newTask7.DueDate);
            Assert.AreEqual(testTask.GetDescription(), newTask7.GetDescription());
        }

        [TestMethod]
        public void TestNaturalTasksWithMissingTasks()
        {
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation("on Wednesday at three PM"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation("at three PM on Wednesday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation(""));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation(", three PM, Wednesday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation(", Wednesday, three PM"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation("three PM Wednesday"));
            Assert.ThrowsExactly<ArgumentException>(() => StringParse.ParseNaturalTaskCreation("Wednesday three PM"));
        }

        #endregion
    }
}