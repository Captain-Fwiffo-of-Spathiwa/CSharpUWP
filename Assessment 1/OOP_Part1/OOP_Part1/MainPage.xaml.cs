using OOP_Part1.Models;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace OOP_Part1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Testing content
        Task Task0 = new("Ima change this later");
        Task Task1 = new("feed Kiwi");
        Task Task2 = new("give Kiwi pats");
        Task Task3 = new("do homewo... oh time for bed");

        TaskList List1 = new("Fun tasks");
        TaskList List2 = new("Meh tasks");

        TaskCollection CollectionOfTasks = new();


        public MainPage()
        {
            InitializeComponent();

            TestTasks();
            TestTaskLists();
            TestCollection();
        }

        private void TestTasks()
        {
            Debug.WriteLine("\n\n\n ============ TESTING TASKS ============ \n\n\n");

            Task0.SetDescription("");
            Task1.ToggleCompletion();
            Task3.ToggleCompletion();
            Task0.Notes = "I know I had a task here somewhere but forgot to name it!";
            Task1.Notes = "Probably walk her first or it's poop city.";

            Debug.WriteLine($"Task 0: {Task0.GetDescription()} is {(Task0.IsComplete ? "complete" : "incomplete")}. Notes: {Task0.Notes}");
            Debug.WriteLine($"Task 1: {Task1.GetDescription()} is {(Task1.IsComplete ? "complete" : "incomplete")}. Notes: {Task1.Notes}");
            Debug.WriteLine($"Task 2: {Task2.GetDescription()} is {(Task2.IsComplete ? "complete" : "incomplete")}. Notes: {Task2.Notes}");
            Debug.WriteLine($"Task 3: {Task3.GetDescription()} is {(Task3.IsComplete ? "complete" : "incomplete")}. Notes: {Task3.Notes}");

            Debug.WriteLine($"Task0 created: {Task0.DateCreated}"); // C# is kind with turning this into a string for no effort.
            Debug.WriteLine($"Task1 created: {Task1.DateCreated}");
            Debug.WriteLine($"Task2 created: {Task2.DateCreated}");
            Debug.WriteLine($"Task3 created: {Task3.DateCreated}");
        }

        private void TestTaskLists()
        {
            Debug.WriteLine("\n\n\n ============ TESTING LISTS ============ \n\n\n");

            TaskList tempList = new("");
            tempList.AddTask(new("walk Kiwi"));
            tempList.AddTask(new("practise piano"));
            tempList.AddTask(new("code the thing"));

            Task tempTask1 = new("buy food");
            Task tempTask2 = new("write entire game");
            tempList.AddTask(tempTask1);
            tempList.AddTask(tempTask2);

            tempTask1.ToggleCompletion();
            tempTask2.ToggleCompletion();


            List1.AddTask(Task1);
            List1.AddTask(Task2);

            List2.AddTask(Task0);
            List2.AddTask(Task3);

            Debug.WriteLine($"List1 has {List1.TotalTasksCount} tasks, of which {List1.IncompleteTasksCount} is/are incomplete.");
            Debug.WriteLine($"List2 has {List2.TotalTasksCount} tasks, of which {List2.IncompleteTasksCount} is/are incomplete.");
            Debug.WriteLine($"The temporary list has {tempList.TotalTasksCount} tasks, of which {tempList.IncompleteTasksCount} is/are incomplete.");

            List1.ClearCompletedTasks();
            List2.ClearCompletedTasks();
            tempList.ClearCompletedTasks();

            Debug.WriteLine($"List1 now has {List1.TotalTasksCount} tasks, of which {List1.IncompleteTasksCount} is/are incomplete.");
            Debug.WriteLine($"List2 now has {List2.TotalTasksCount} tasks, of which {List2.IncompleteTasksCount} is/are incomplete.");
            Debug.WriteLine($"The temporary list now has {tempList.TotalTasksCount} tasks, of which {tempList.IncompleteTasksCount} is/are incomplete.");
        }

        private void TestCollection()
        {
            Debug.WriteLine("\n\n\n ============ TESTING COLLECTION ============ \n\n\n");

            TaskList tempList = new("");
         
            Task tempTask1 = new("buy food");
            Task tempTask2 = new("write entire game");
            tempList.AddTask(tempTask1);
            tempList.AddTask(tempTask2);

            tempTask1.ToggleCompletion();

            CollectionOfTasks.AddTaskList(List1);
            CollectionOfTasks.AddTaskList(List2);
            CollectionOfTasks.AddTaskList(tempList);

            Debug.WriteLine($"The tasks collection has {CollectionOfTasks.TotalTasksCount} tasks in total, of which {CollectionOfTasks.IncompleteTasksCount} are incomplete.");

            CollectionOfTasks.ClearAllCompletedTasks();

            Debug.WriteLine($"After cleaning, the tasks collection now has {CollectionOfTasks.TotalTasksCount} tasks in total, of which {CollectionOfTasks.IncompleteTasksCount} are still incomplete.");
        }
    }
}
