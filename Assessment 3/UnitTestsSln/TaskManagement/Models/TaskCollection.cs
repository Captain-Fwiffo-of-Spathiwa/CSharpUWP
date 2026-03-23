using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Helpers;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using File = System.IO.File;


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
}
}
