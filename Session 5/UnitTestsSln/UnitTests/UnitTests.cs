using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Windows.Storage;
using TaskManagement.Helpers;
using TaskManagement.Models;
using Windows.ApplicationModel.Activation;

/*

The unit tests should check that the following work as expected:

[✓] Adding a new list to the collection.
[✓] Adding tasks to a list. Don’t forget to check the task count.
[✓] Completing a task. Don’t forget to check the count of complete and
    incomplete tasks as part of this.
[✓] Setting a task to be incomplete, Don’t forget to check the count of
    complete and incomplete tasks as part of this.
[✓] Deleting tasks from a list, including emptying the list. Don’t forget
    to check the task count as part of this.
[✓] Test that repeating tasks repeat correctly.
[✓] Test that habits correctly count completions.
[✓] Test the percentage complete for projects.

The unit tests should check that the app handles the following errors:

[✓] Settings the task name to be blank.
[✓] Setting the list name to be blank.
[✓] Placing a repeating task in a project.
[✓] Placing a habit in a project.
[✓] A repeating task with incomplete information (eg. Missing schedule)

*/



namespace UnitTests
{
    /// <summary>
    /// Unit tests for TaskCollections, TaskLists, Tasks, and their subtypes.
    /// Last run: March 23rd, 2026
    /// 
    /// Changelog:
    /// ----------
    /// 24/03/26: Fix RepeatingTask completion timings.
    /// 23/03/26: Collection, List, and Task tests added. Fixed TestSaveAndLoadTaskCollection().
    /// 16/03/06: Binary save and load tests added.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        string        TestBinarySaveFilename;
        StorageFolder TestFolder;
        StorageFile   TestFile;

        TaskManagement.Models.Task          TaskA;
        TaskManagement.Models.Task          TaskB;
        TaskManagement.Models.RepeatingTask RepTaskA;
        TaskManagement.Models.RepeatingTask RepTaskB;
        TaskManagement.Models.Habit         HabitA;
        TaskManagement.Models.Habit         HabitB;


        #region Assessment 3 test members

        TaskManagement.Models.TaskList       AT3TasksListA;
        TaskManagement.Models.TaskList       AT3TasksListB;
        TaskManagement.Models.TaskCollection AT3TasksCollection;

        #endregion


        #region Assessment 4 test members

        TaskManagement.Models.TaskList       AT4TasksListA;
        TaskManagement.Models.TaskList       AT4TasksListB;
        TaskManagement.Models.TaskCollection AT4TasksCollection;

        #endregion

        /// <summary>
        /// Setup class fields used by our tests.
        /// </summary>
        [TestInitialize]
        public async System.Threading.Tasks.Task Setup()
        {
            /* ------------------------------------------------------------ 
             *  A constructor could do this work too, but [TestInitialize]
             *  means we get exceptions handled by reporting them as test
             *  failures.
             * -----------------------------------------------------------*/

            /* ------------------------------------------------------------ 
             *  We want this file creation to be async, because the code
             *  being tested uses async. With UnitTests, async functions
             *  need to return a Task so that the test framework can
             *  await/work with those Tasks properly.
             *  
             *  Yes, we have our own type called "Task".
             *  That's just unfortunate.
             * -----------------------------------------------------------*/

            TestBinarySaveFilename = "TestBinarySaveFile.bin";
            TestFolder = ApplicationData.Current.LocalFolder;
            TestFile = await TestFolder.CreateFileAsync(TestBinarySaveFilename, CreationCollisionOption.ReplaceExisting);

            TaskA = new("Buy cats");
            TaskB = new("Buy dogs");
            RepTaskA = new("Tend to cats", DateTime.Now, TaskManagement.Models.Frequency.Weekly);
            RepTaskB = new("Tend to dogs", DateTime.Now, TaskManagement.Models.Frequency.Daily);
            HabitA = new("Pat dogs", DateTime.Now, TaskManagement.Models.Frequency.Daily, 5);
            HabitB = new("Ignore cats", DateTime.Now, TaskManagement.Models.Frequency.Daily, 10);


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


        #region Assessment 4 Tests

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
                     *  We could binary load back into the received task
                     *  object, but that wouldn't make for a very
                     *  convincing test. We want a whole new object instead
                     *  so we're sure the tested data is the loaded data.
                     *  This means we need to know the object's subtype.
                     * ---------------------------------------------------*/

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
    }
}
