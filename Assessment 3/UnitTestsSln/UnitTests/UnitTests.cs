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
[] Test that habits correctly count completions.
[] Test the percentage complete for projects.

The unit tests should check that the app handles the following errors:

[] Settings the task name to be blank.
[] Setting the list name to be blank.
[] Placing a repeating task in a project.
[] Placing a habit in a project.
[] A repeating task with incomplete information (eg. Missing schedule)

*/



namespace UnitTests
{
    /// <summary>
    /// Unit tests for TaskCollections, TaskLists, Tasks, and their subtypes.
    /// Last run: March 23rd, 2026
    /// 
    /// Changelog:
    /// ----------
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

        TaskManagement.Models.TaskList AT4TasksListA;
        TaskManagement.Models.TaskList AT4TasksListB;
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
            HabitA = new("Pat dogs", DateTime.Now, TaskManagement.Models.Frequency.Daily, 0);
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

        /// <summary>
        /// Test collection addition of an empty list, populated lists, and a
        /// list with tasks added after the list was put in a collection.
        /// </summary>
        [TestMethod]
        public void TestAddNewListToCollection()
        {
            Assert.AreEqual(0, AT3TasksCollection.TotalTasksCount);
            Assert.AreEqual("\n ------------------------ \n", AT3TasksCollection.ToString());

            AT3TasksCollection.AddTaskList(AT3TasksListA);
            Assert.AreEqual(0, AT3TasksCollection.TotalTasksCount);

            AT3TasksListA.AddTask(TaskA);
            Assert.AreEqual(1, AT3TasksCollection.TotalTasksCount);

            AT3TasksListB.AddTask(TaskA);
            AT3TasksListB.AddTask(HabitA);
            AT3TasksCollection.AddTaskList(AT3TasksListB);
            Assert.AreEqual(3, AT3TasksCollection.TotalTasksCount);

            String expectedString = "Task: Buy cats\nTask: Buy cats\nHabit: Pat dogs\n\n ------------------------ \n";
            Assert.AreEqual(expectedString, AT3TasksCollection.ToString());
        }

        /// <summary>
        /// Test adding singles, duplicates, subtyped Tasks to a TaskList.
        /// </summary>
        [TestMethod]
        public void TestAddTasksToList()
        {
            Assert.AreEqual(0, AT3TasksListA.TotalTasksCount);

            AT3TasksListA.AddTask(TaskA);
            Assert.AreEqual(1, AT3TasksListA.TotalTasksCount);

            AT3TasksListA.AddTask(TaskA);
            Assert.AreEqual(2, AT3TasksListA.TotalTasksCount);

            AT3TasksListA.AddTask(RepTaskA);
            AT3TasksListA.AddTask(HabitB);
            Assert.AreEqual(4, AT3TasksListA.TotalTasksCount);

            String expected = "Task: Buy cats\nTask: Buy cats\nRepeating Task: Tend to cats\nHabit: Ignore cats\n";
            Assert.AreEqual(expected, AT3TasksListA.ToString());
        }

        /// <summary>
        /// Test completing tasks individually and plus a TaskList's
        /// recognition of completed Tasks.
        /// </summary>
        [TestMethod]
        public void TestCompleteTasks()
        {
            Assert.AreEqual(0, AT3TasksListA.IncompleteTasksCount);

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
            Assert.IsTrue(TaskA.IsComplete);
            Assert.IsTrue(RepTaskA.IsComplete);
            Assert.AreEqual(4, AT3TasksListA.IncompleteTasksCount);
            Assert.AreEqual(6, AT3TasksListA.TotalTasksCount);

            AT3TasksListA.ClearCompletedTasks();
            Assert.AreEqual(4, AT3TasksListA.TotalTasksCount);
        }

        /// <summary>
        /// Test setting complete tasks as incomplete individually, plus
        /// the TaskList's abiltiy to not recognise and not remove incomplete
        /// Tasks.
        /// </summary>
        [TestMethod]
        public void TestSetTaskIncomplete()
        {
            Assert.AreEqual(0, AT3TasksListA.IncompleteTasksCount);

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
            Assert.IsFalse(TaskA.IsComplete);
            Assert.IsFalse(RepTaskA.IsComplete);
            Assert.AreEqual(2, AT3TasksListA.IncompleteTasksCount);
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
        /// Test that a RepeatingTask repeats; that is, it updates its DueDate
        /// if the present day is inside its next frequency window.
        /// </summary>
        [TestMethod]
        public void TestRepeatingTasksRepeat()
        {
            int daysInPast = 11;    
            int daysInFuture = 12;

            RepeatingTask pastRepDailyTask = new RepeatingTask("pastRepDailyTask", DateTime.Now - TimeSpan.FromDays(daysInPast), Frequency.Daily);
            RepeatingTask pastRepWeeklyTask = new RepeatingTask("pastRepWeeklyTask", DateTime.Now - TimeSpan.FromDays(daysInPast), Frequency.Weekly);
            RepeatingTask presentRepDailyTask = new RepeatingTask("presentRepDailyTask", DateTime.Now, Frequency.Daily);
            RepeatingTask presentRepWeeklyTask = new RepeatingTask("presenttRepWeeklyTask", DateTime.Now, Frequency.Weekly);
            RepeatingTask futureRepDailyTask = new RepeatingTask("futureRepDailyTask", DateTime.Now + TimeSpan.FromDays(daysInFuture), Frequency.Daily);
            RepeatingTask futureRepWeeklyTask = new RepeatingTask("futureRepWeeklyTask", DateTime.Now + TimeSpan.FromDays(daysInFuture), Frequency.Weekly);

            Assert.IsFalse(pastRepDailyTask.IsComplete);
            Assert.IsFalse(pastRepWeeklyTask.IsComplete);
            Assert.IsFalse(presentRepDailyTask.IsComplete);
            Assert.IsFalse(presentRepWeeklyTask.IsComplete);
            Assert.IsFalse(futureRepDailyTask.IsComplete);
            Assert.IsFalse(futureRepWeeklyTask.IsComplete);

            pastRepDailyTask.ToggleCompletion();
            pastRepWeeklyTask.ToggleCompletion();
            presentRepDailyTask.ToggleCompletion();
            presentRepWeeklyTask.ToggleCompletion();
            futureRepDailyTask.ToggleCompletion();
            futureRepWeeklyTask.ToggleCompletion();

            // Completing tasks that were due in the past will get a new due date that is at the
            // end of the due date window that fits the present day.
            // Eg: A weekly task made 11 days ago will get a new due date of 3 days in the future.
            // Eg: A daily task made any time in the past will get a new due date of tomorrow.
            String nextDailyDueDate = (DateTime.Now + TimeSpan.FromDays(1)).ToShortDateString();
            int daysAheadForNextWeeklyDueDate = 7 - (daysInPast % 7);  // 7 - (11 % 7) == 3
            String nextWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(daysAheadForNextWeeklyDueDate)).ToShortDateString();
            Assert.AreEqual(nextDailyDueDate, pastRepDailyTask.DueDate.Value.ToShortDateString());
            Assert.AreEqual(nextWeeklyDueDate, pastRepWeeklyTask.DueDate.Value.ToShortDateString());

            // Completing tasks that were due today will get a new due date for the due date window
            // that starts today.
            nextWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(7)).ToShortDateString();
            Assert.AreEqual(nextDailyDueDate, presentRepDailyTask.DueDate.Value.ToShortDateString());
            Assert.AreEqual(nextWeeklyDueDate, presentRepWeeklyTask.DueDate.Value.ToShortDateString());

            // Completing tasks due in the future will not get a new due date until the present date
            // passes that future due date.
            String nextFutureDailyDueDate = (DateTime.Now + TimeSpan.FromDays(daysInFuture)).ToShortDateString();
            String nextFutureWeeklyDueDate = (DateTime.Now + TimeSpan.FromDays(daysInFuture)).ToShortDateString();
            Assert.AreEqual(nextFutureDailyDueDate, futureRepDailyTask.DueDate.Value.ToShortDateString());
            Assert.AreEqual(nextFutureWeeklyDueDate, futureRepWeeklyTask.DueDate.Value.ToShortDateString());

            // Ensure RepeatedTasks completed today report as complete since today is inside or
            // before the next due date window.
            Assert.IsTrue(pastRepDailyTask.IsComplete);
            Assert.IsTrue(pastRepWeeklyTask.IsComplete);
            Assert.IsTrue(presentRepDailyTask.IsComplete);
            Assert.IsTrue(presentRepWeeklyTask.IsComplete);
            Assert.IsTrue(futureRepDailyTask.IsComplete);
            Assert.IsTrue(futureRepWeeklyTask.IsComplete);
        }

        [TestMethod]
        public void TestHabitsCountCompletions()
        { Assert.Fail(); }

        [TestMethod]
        public void TestProjectsCompletePercentage()
        { Assert.Fail(); }

        [TestMethod]
        public void TestBadTaskName()
        { Assert.Fail(); }

        [TestMethod]
        public void TestBadListName()
        { Assert.Fail(); }

        [TestMethod]
        public void TestAddRepeatingTaskToProject()
        { Assert.Fail(); }

        [TestMethod]
        public void TestAddHabitToProject()
        { Assert.Fail(); }

        [TestMethod]
        public void TestRepeatingTaskWithMissingInformation()
        { Assert.Fail(); }
























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
