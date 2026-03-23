using TaskManagement.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace TaskManagement.Models
{
    public class TaskList
    {
        private const string DefaultName = "Just another TaskList";

        private string      Name;
        private List<Task>  Tasks = new();

        // Lambda operator saying "Just return whatever this lne evaluates to". No setter desired.
        public int TotalTasksCount => Tasks.Count;
        public int IncompleteTasksCount
        {
            get
            {
                int incomplete = 0;
                foreach (var task in Tasks)
                {
                    if (!task.IsComplete)
                    {
                        ++incomplete;
                    }
                }

                return incomplete;
            }
        }


        public TaskList(string name)
        {
            SetName(name);
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                System.Diagnostics.Debug.WriteLine("Error - Blank TaskList name given.");
                Name = DefaultName;
                return;
            }

            Name = name;
        }

        public virtual void AddTask(Task task)
        {
            Tasks.Add(task);    // Duplicate pointers welcome
        }

        public void ClearCompletedTasks()
        {
            // This is nice. "Remove each item where some predicate based on that item is true".
            // The alternative I would have used would be a reverse indexed for loop, since C#
            // doesn't let you modify a list in place with iterators.
            Tasks.RemoveAll(task => task.IsComplete);
        }

        // Assessment 4 additions
        public void SaveTo(BinaryWriter writer)
        {
            SaveUtils.SaveAndPrintString(writer, Name);
            SaveUtils.SaveAndPrintInt(writer, Tasks.Count);

            foreach (var task in Tasks)
            {
                task.SaveTo(writer);
            }
        }

        /// <summary>
        /// Destructively attempted a binary load. If the load fails, this
        /// object's current data will be destroyed in the process.
        /// </summary>
        /// <param name="reader"></param>
        public void LoadFrom(BinaryReader reader)
        {
            Name = "";
            Tasks = new();

            // Read the name of this TaskList, then the number of Tasks it's
            // expected to have, then read the subtype of each Task before
            // attempting to load that Task subtype.
            Name = SaveUtils.LoadAndPrintString(reader);
            int numTasks = SaveUtils.LoadAndPrintInt(reader);

            for (int i = 0; i < numTasks; ++i)
            {
                int taskType = SaveUtils.LoadAndPrintInt(reader);

                switch (taskType)
                {
                    // We use dummy object values for the new Tasks we'll be overwriting.
                    // We could write default constructors instead, but this will do.
                    case 0:
                        Tasks.Add(new Task("temp Task"));
                        break;

                    case 1:
                        Tasks.Add(new RepeatingTask("temp Repeating Task", DateTime.Now, Frequency.Daily));
                        break;

                    case 2:
                        Tasks.Add(new Habit("temp Habit", DateTime.Now, Frequency.Daily, 0));
                        break;

                    default:
                        break;
                }

                Tasks.Last().LoadFrom(reader);
            }
        }

        public override string ToString()
        {
            string outString = string.Empty;

            foreach (var task in Tasks)
            {
                outString += task.ToString();
            }

            return outString;
        }
    }
}
