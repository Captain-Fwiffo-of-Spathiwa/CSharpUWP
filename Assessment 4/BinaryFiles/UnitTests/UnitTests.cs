using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Windows.Storage;
using BinaryFiles.Helpers;
using BinaryFiles.Models;



namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        string TestBinarySaveFilename;
        StorageFolder TestFolder;
        StorageFile TestFile;

        BinaryFiles.Models.Task TaskA;
        BinaryFiles.Models.Task TaskB;
        BinaryFiles.Models.RepeatingTask RepTaskA;
        BinaryFiles.Models.RepeatingTask RepTaskB;
        BinaryFiles.Models.Habit HabitA;
        BinaryFiles.Models.Habit HabitB;

        BinaryFiles.Models.TaskList TasksListA;
        BinaryFiles.Models.TaskList TasksListB;

        BinaryFiles.Models.TaskCollection TasksCollection;


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
            RepTaskA = new("Tend to cats", DateTime.Now, BinaryFiles.Models.Frequency.Weekly);
            RepTaskB = new("Tend to dogs", DateTime.Now, BinaryFiles.Models.Frequency.Daily);
            HabitA = new("Pat dogs", DateTime.Now, BinaryFiles.Models.Frequency.Daily, 0);
            HabitB = new("Ignore cats", DateTime.Now, BinaryFiles.Models.Frequency.Daily, 10);

            TasksListA = new("List of Tasks");
            TasksListB = new("Other list of Tasks");

            TasksCollection = new();

            TasksListA.AddTask(TaskA);
            TasksListA.AddTask(RepTaskA);
            TasksListA.AddTask(HabitA);

            TasksListB.AddTask(TaskB);
            TasksListB.AddTask(RepTaskB);
            TasksListB.AddTask(HabitB);

            TasksCollection.AddTaskList(TasksListA);
            TasksCollection.AddTaskList(TasksListB);

            Assert.AreEqual(TasksListA.TotalTasksCount, 3);
            Assert.AreEqual(TasksListB.TotalTasksCount, 3);

            Assert.AreEqual(TasksCollection.TotalTasksCount, 6);
            Assert.AreEqual(TasksCollection.IncompleteTasksCount, 6);
        }

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

        private void TestBinarySaveAndLoadGivenTaskType(BinaryFiles.Models.Task task)
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

                    BinaryFiles.Models.Task newTask;
                    int newTaskExpectedTypeID;

                    // The order of this if-chain matters, due to the chain of inheritance.
                    // Ie., don't ask a Habit if it's just a Task.
                    if (task is BinaryFiles.Models.Habit)
                    {
                        // new() lets us cast to a subtype
                        newTask = new BinaryFiles.Models.Habit("dummy desc", DateTime.Now, BinaryFiles.Models.Frequency.Daily, 0);
                        newTaskExpectedTypeID = 2;
                    }
                    else if (task is BinaryFiles.Models.RepeatingTask)
                    {
                        newTask = new BinaryFiles.Models.RepeatingTask("dummy desc", DateTime.Now, BinaryFiles.Models.Frequency.Daily);
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
                    TasksListA.SaveTo(writer);
                    TasksListB.SaveTo(writer);
                }
            }

            using (var stream = File.Open(TestFolder.Path + "\\" + TestBinarySaveFilename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    // We don't have subtypes and a TaskList save manages
                    // its own count, so there's nothing complex here.
                    BinaryFiles.Models.TaskList newTasksListA = new("dummy desc");
                    newTasksListA.LoadFrom(reader);
                    Assert.AreEqual(TasksListA.ToString(), newTasksListA.ToString());

                    BinaryFiles.Models.TaskList newTasksListB = new("dummy desc");
                    newTasksListB.LoadFrom(reader);
                    Assert.AreEqual(TasksListB.ToString(), newTasksListB.ToString());
                }
            }
        }

        /// <summary>
        /// Test binary save and load of a TaskCollection.
        /// </summary>
        [TestMethod]
        public void TestBinarySaveAndLoadTaskCollection()
        {
            // TaskCollections take care of their own binary reader and
            // writer so testing them is quite mindless.
            TasksCollection.Save();
            TaskCollection newTasksCollection = new();
            newTasksCollection.Load();

            Assert.AreEqual(TasksCollection.ToString(), newTasksCollection.ToString());
        }
    }
}
