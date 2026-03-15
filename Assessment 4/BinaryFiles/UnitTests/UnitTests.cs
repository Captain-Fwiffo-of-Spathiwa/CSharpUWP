using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFiles;
using System;
using Windows.ApplicationModel.UserDataTasks.DataProvider;


// Unfortunately, there are a couple of common problems caused by Visual Studio that crop up with unit tests.
// • If the tests don’t pass or fail, build and run the app (not the tests) and try again.
// • If you get an error message saying “Payload contains two or more files with the same destination path
// 'Properties\Default.rd.xml', but they are different sizes”, delete the Properties\Default.rd.xml file in your unit test project. 



namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        BinaryFiles.Models.Task             TaskA;
        BinaryFiles.Models.Task             TaskB;
        BinaryFiles.Models.RepeatingTask    RepTaskA;
        BinaryFiles.Models.RepeatingTask    RepTaskB;
        BinaryFiles.Models.Habit            HabitA;
        BinaryFiles.Models.Habit            HabitB;

        BinaryFiles.Models.TaskList         TasksListA;
        BinaryFiles.Models.TaskList         TasksListB;

        BinaryFiles.Models.TaskCollection   TasksCollection;

        /// <summary>
        /// Setup class fields used by our tests.
        /// A constructor could do this too, but using [TestInitialize] means we get exceptions
        /// handled by reporting them as test failures.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TaskA           = new("Buy cats");
            TaskB           = new("Buy dogs");
            RepTaskA        = new("Tend to cats", DateTime.Now, BinaryFiles.Models.Frequency.Weekly);
            RepTaskB        = new("Tend to dogs", DateTime.Now, BinaryFiles.Models.Frequency.Daily);
            HabitA          = new("Pat dogs", DateTime.Now, BinaryFiles.Models.Frequency.Daily, 0);
            HabitB          = new("Ignore cats", DateTime.Now, BinaryFiles.Models.Frequency.Daily, 10);
    
            TasksListA      = new("List of Tasks");
            TasksListB      = new("Other list of Tasks");
    
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


        [TestMethod]
        public void TestBinarySaveTask()
        {

        }












        //// Use the 'UITestMethod' attribute for tests that need to run on the UI thread
        //[UITestMethod]
        //public void TestMethod2()
        //{
        //    Grid grid = new();

        //    Assert.AreEqual(0, grid.MinWidth);
        //}
    }
}
