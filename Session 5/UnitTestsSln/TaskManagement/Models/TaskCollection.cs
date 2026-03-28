using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TaskManagement.Helpers;
using Windows.Storage;
using File = System.IO.File;

/* ------------------------------------------------------------------------
    Session 5 Assessment Task

    Add the following methods to your task collection class:
        [] ... returns a list of all tasksToSort, sorted by name
        [] ... returns a list of all tasksToSort, sorted by due date
        [] ... returns a list of all tasksToSort, sorted by creation date
        [] ... returns a list of all tasksToSort, sorted by priority
        [] ... returns all habits
        [] ... returns all repeating tasksToSort
        [] ... returns a list of all tasksToSort due today
        [] ... returns all tasksToSort with a given description

    Note that some of these may require you to make equivalent methods for
    the task list class. It depends on how you implement them.

    Testing
        [] Write unit tests to show that everything is working correctly
        
    You may also wish to create some code that outputs to the console just
    for your own testing as you do this exercise, but that is not required.
   ------------------------------------------------------------------------*/



namespace TaskManagement.Models
{
public class TaskCollection
{
    private List<TaskList>  TaskLists = new();

    public int TotalTasksCount
    {
        get
        {
            int total = 0;
            foreach (var taskList in TaskLists)
            {
                total += taskList.TotalTasksCount;
            }

            return total;
        }
    }

    public int IncompleteTasksCount
    {
        get
        {
            int incomplete = 0;
            foreach (var taskList in TaskLists)
            {
                incomplete += taskList.IncompleteTasksCount;
            }

            return incomplete;
        }
    }

    public void AddTaskList(TaskList taskList)
    {
        TaskLists.Add(taskList);
    }

    public void ClearAllCompletedTasks()
    {
        foreach (var taskList in TaskLists)
        {
            taskList.ClearCompletedTasks();
        }
    }

    public async System.Threading.Tasks.Task Save(String binarySaveFilename)
    {
        // We use the app's local folder and create a file there
        Debug.WriteLine($"Attempting to save to:\n\t" +
            $"{ApplicationData.Current.LocalFolder.Path}");

        // We handle exceptions in a very broad stroke, outside the using statements
        try
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync(binarySaveFilename, CreationCollisionOption.ReplaceExisting);

            using (var stream = File.Open(file.Path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    SaveUtils.SaveAndPrintInt(writer, TaskLists.Count);

                    foreach (var taskList in TaskLists)
                    {
                        taskList.SaveTo(writer);
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Could not open {binarySaveFilename} for writing. Disk or file error.");
        }
        catch (UnauthorizedAccessException ex)
        {
            // We can check this by simply setting the file to read-only
            Debug.WriteLine($"Could not open {binarySaveFilename} for writing. ACCESS DENIED.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving data. Exception message: {ex.Message}");
            // We could kill the application here, since our using blocks have at
            // least seen the file/stream resources disposed of. But in this case
            // we'll let the application continue to run even if it couldn't save.
            // If we were to call CoreApplication.Kill(), it's not an immediate
            // out. Subsequent code will still run until Windows shuts the app
            // down itself.
            // CoreApplication.Kill();
        }
    }

    /// <summary>
    /// Destructively attempted a binary load. If the load fails, this
    /// object's current data will be destroyed in the process.
    /// </summary>
    public void Load(String binarySaveFilename)
    {
        TaskLists = new();

        Debug.WriteLine($"Attempting to load from:\n\t" +
            $"{ApplicationData.Current.LocalFolder.Path}");

        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        try
        {
            using (var stream = File.Open(storageFolder.Path + "\\" + binarySaveFilename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    int numTaskLists = SaveUtils.LoadAndPrintInt(reader);

                    for (int i = 0; i < numTaskLists; ++i)
                    {
                        TaskLists.Add(new("temp TaskList"));
                        TaskLists.Last().LoadFrom(reader);
                    }
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            // We can check this with a breakpoint, manually deleting the file, then continuing
            Debug.WriteLine($"Could not open {binarySaveFilename} for reading. File not found.");
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Could not open {binarySaveFilename} for reading. Disk or file error.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading data. Exception message: {ex.Message}");
        }
    }

    public void PrintAllTaskListTasks()
    {
        foreach (var taskList in TaskLists)
        {
            Debug.WriteLine(taskList.ToString());
        }

        Debug.WriteLine("\n ------------------------ \n");
    }

    public override string ToString()
    {
        String collectionString = string.Empty;

        foreach (var taskList in TaskLists)
        {
            collectionString += taskList.ToString();
        }

        collectionString += "\n ------------------------ \n";

        return collectionString;
    }

    public List<Task> GetTasksSortedByName()
    {
        /* ---------------------------------------------------------------
            There's not much point in letting our Sort go down to the
            TaskLists themselves, since the TaskCollection has multiple
            TaskLists that would still need futher sorting to combine.

            So we'll build a copy of references first, then Sort that.

            It's not hard to have a thousand TaskLists and squillions of
            different Tasks. So we allocate some space first intead of
            worrying about the list lurching around resizing at runtime.
            --------------------------------------------------------------*/
        List<Task> tasksToSort = GetAllTasks();
        tasksToSort.Sort((task1, task2) => task1.GetDescription().CompareTo(task2.GetDescription()));
     
        foreach(var task in tasksToSort)
        {
            Debug.WriteLine(task.ToString());
        }
            
        return tasksToSort;
    }

    private List<Task> GetAllTasks()
    {
        int numTasks = 0;
        foreach (var tasklist in TaskLists)
        {
            numTasks += tasklist.TotalTasksCount;
        }

        List<Task> sortedTasks = new List<Task>(numTasks);

        foreach (var tasklist in TaskLists)
        {
            sortedTasks.AddRange(tasklist.GetTasks());
        }

        return sortedTasks;
    }

}
}
