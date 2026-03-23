using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;
using File = System.IO.File;
using System.Linq;
using BinaryFiles.Helpers;
using Windows.ApplicationModel.Core;

/*
[✓] Update your task collection class to have a save() and a load() method
    that handles storing the task lists and tasks. 

[✓] The task collection itself, however, is just there to hold everything else
    and has noneof its own data that needs to be saved.

[✓] Ensure the app fails gracefully if there is a problem.

Testing
=======

[✓] Write some code to show that your classes, methods and properties are all
    working as intended and output the results to the console.

[✓] Make sure the app fails gracefully when it runs into problems - for example,
   if the file is missing.

[✓] Also create unit tests to check that saving and loading returns the same
    data, including samples of each type of data (habits, projects, etc). Don’t
    forget that race conditions may affect this, depending on how it’s done. 
*/

namespace BinaryFiles.Models
{
public class TaskCollection
{
    private List<TaskList>  TaskLists = new();
    private string BinarySaveFilename = "MyTasksCollection.bin";

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

    public async void Save()
    {
        // We use the app's local folder and create a file there
        Debug.WriteLine($"Attempting to save to:\n\t" +
            $"{ApplicationData.Current.LocalFolder.Path}");

        // We handle exceptions in a very broad stroke, outside the using statements
        try
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync(BinarySaveFilename, CreationCollisionOption.ReplaceExisting);

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
            Debug.WriteLine($"Could not open {BinarySaveFilename} for writing. Disk or file error.");
        }
        catch (UnauthorizedAccessException ex)
        {
            // We can check this by simply setting the file to read-only
            Debug.WriteLine($"Could not open {BinarySaveFilename} for writing. ACCESS DENIED.");
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
    public void Load()
    {
        TaskLists = new();

        Debug.WriteLine($"Attempting to load from:\n\t" +
            $"{ApplicationData.Current.LocalFolder.Path}");

        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        try
        {
            using (var stream = File.Open(storageFolder.Path + "\\" + BinarySaveFilename, FileMode.Open))
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
            Debug.WriteLine($"Could not open {BinarySaveFilename} for reading. File not found.");
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Could not open {BinarySaveFilename} for reading. Disk or file error.");
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
}
}
