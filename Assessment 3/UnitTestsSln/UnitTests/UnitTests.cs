using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Windows.Storage;
using TaskManagement.Helpers;
using TaskManagement.Models;

/*

The unit tests should check that the following work as expected:

[✓] Adding a new list to the collection.
[] Adding tasks to a list. Don’t forget to check the task count.
[] Completing a task. Don’t forget to check the count of complete and
   incomplete tasks as part of this.
[] Setting a task to be incomplete, Don’t forget to check the count of
   complete and incomplete tasks as part of this.
[] Deleting tasks from a list, including emptying the list. Don’t forget
   to check the task count as part of this.
[] Test that repeating tasks repeat correctly.
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
    /// 
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
        /// Test adding new lists to a collection.
        ///
        /// Checks addition of an empty list, a populated lists, and a list
        /// with tasks added after the list was put in a collection.
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
        /// 
        /// </summary>
        [TestMethod]
        public void TestAddTasksToList()
        { }





        [TestMethod]
        public void TestCompleteTask()
        { }

        [TestMethod]
        public void TestSetTaskIncomplete()
        { }

        [TestMethod]
        public void TestDeleteTasksFromList()
        { }

        [TestMethod]
        public void TestRepeatingTasksRepeat()
        { }

        [TestMethod]
        public void TestHabitsCountCompletions()
        { }

        [TestMethod]
        public void TestProjectsCompletePercentage()
        { }

        [TestMethod]
        public void TestBadTaskName()
        { }

        [TestMethod]
        public void TestBadListName()
        { }

        [TestMethod]
        public void TestAddRepeatingTaskToProject()
        { }

        [TestMethod]
        public void TestAddHabitToProject()
        { }

        [TestMethod]
        public void TestRepeatingTaskWithMissingInformation()
        { }





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
