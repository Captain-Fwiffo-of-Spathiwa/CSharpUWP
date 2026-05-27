
# Task Class

Representation of a job that needs doing with an optional due date. Tasks can be toggled complete, prioritised and have notes attached. Where applicable, a Task's overdue status can be queried.

## Definition

```
public class Task
```

- Namespace  
    - TaskManagements.Models

## Fields

| Name                    | Description                                                        |
|-------------------------|--------------------------------------------------------------------|
| Notes (string)          | Descriptive notes about the Task.                                  |
| DateCreated (DateTime)  | The date and time when the Task was created.                       |
| TaskPriority (Priority) | The priority level of the Task. Lower value means higher priority. |
| DueDate (DateTime)      | The due date for the Task, if any.                                 |
| IsComplete (bool)       | True if the Task is marked as complete.                            |
| Overdue (bool)          | True if the task is overdue, false otherwise.                      |

## Methods

| Name                   | Description                               |
|------------------------|-------------------------------------------|
| Task(string)           | Construct a new Task with a description.  |
| GetDescription()       | Get the description of the Task.          |
| SetDescription(string) | Set the description of the Task.          |
| ToggleCompletion()     | Toggle the completion state of the Task.  |
| SaveTo(BinaryWriter)   | Save the Task to a binary format.         |
| LoadFrom(BinaryWriter) | Load the Task from a binary format.       |
| ToString()             | Return a text representation of the Task. |

_____________________________________________________________

# RepeatingTask Class

A daily or weekly repeating task.

## Definition

```
public class RepeatingTask
```

- Namespace
    - TaskManagement.Models
- Inheritance
    - Task


## Fields

| Name                | Description                      |
|---------------------|----------------------------------|
| Frequency (Frequency) | How often the Task repeats.    |
| IsComplete (bool)     | True if the Task is complete.  |

## Methods

| Name                                       | Description                                                                               |
|--------------------------------------------|-------------------------------------------------------------------------------------------|
| RepeatingTask(string, DateTime, Frequency) | Construct a new RepeatingTask with a given due date and frequency.                     |
| ToggleCompletion()                         | Mark an incomplete RepeatingTask as completed or a completed RepeatingTask as incomplete. |
| SaveTo(BinaryWriter)                       | Save to binary format.                                                                    |
| LoadFrom(BinaryReader)                     | Load from binary format.                                                                  |
| ToString()                                 | Return a text representation.                                                             |

_____________________________________________________________

# Habit Class

Regularly repeating tasks that track completion streaks.

## Definition

```
public class Habit
```

- Namespace
    - TaskManagement.Models
- Inheritance
    - RepeatingTask


## Fields

| Name                                | Description                                                                                          |
|-------------------------------------|------------------------------------------------------------------------------------------------------|
| Streak (int)                        | Number of times a Habit has been completed consecutively.                                            |

## Methods

| Name                                    | Description                                                              |
|-----------------------------------------|--------------------------------------------------------------------------|
| Habit(string, DateTime, Frequency, int) | Construct a new Habit with a given due date and expected frequency.      |
| ToggleCompletion()                      | Mark an incomplete Habit as completed or a completed Habit as incomplete.|
| SaveTo(BinaryWriter)                    | Save a Habit to binary format.                                           |
| LoadFrom(BinaryReader)                  | Load a Habit from binary format.                                         |
| ToString()                              | Return a text representation.                                            |

_____________________________________________________________

# Priority Struct

The priority of a Task. Lower value means a higher priority.

## Definition

```
public struct Priority
```

- Namespace  
    - TaskManagements.Models

## Fields

| Name         | Description            |
|--------------|------------------------|
| Value (int)  | The Priority's value.  |

## Methods

| Name                   | Description                                                     |
|------------------------|-----------------------------------------------------------------|
| Priority(int)          | Create a Priority with the given value.                         |
| operator++(Priority)   | Increment the Priority's value, thus reducing its importance.   |
| operator--(Priority)   | Decrement the Priority's value, thus increasing its importance. |

_____________________________________________________________

# TaskList Class

A collection of Tasks with a single identifying name.

## Definition

```
public class TaskList
```

- Namespace  
    - TaskManagements.Models

## Fields

| Name                       | Description                                     |
|----------------------------|-------------------------------------------------|
| TotalTasksCount (int)      | The total number of Tasks in the TaskList.      |
| IncompleteTasksCount (int) | The number of incomplete Tasks in the TaskList. |

## Methods

| Name                   | Description                                    |
|------------------------|------------------------------------------------|
| TaskList(string)       | Construct a new TaskList with a name.          |
| GetName()              | Get the name of the TaskList.                  |
| SetName(string)        | Set the name of the TaskList.                  |
| AddTask(Task)          | Add a Task to the TaskList.                    |
| ClearCompletedTasks()  | Remove all completed Tasks from the TaskList.  |
| SaveTo(BinaryWriter)   | Save the TaskList to binary format.            |
| LoadFrom(BinaryReader) | Load the TaskList from binary format.          |
| ToString()             | Return a text representation of the TaskList.  |
| GetTasks()             | Get a read-only list of Tasks in the TaskList. |
_____________________________________________________________

# Project Class

A collection of Tasks that do not repeat, that maintains a completion percentage.

## Definition

```
public class Project
```

- Namespace  
    - TaskManagements.Models

## Fields

| Name                     | Description                                           |
|--------------------------|-------------------------------------------------------|
| PercentComplete (float)  | Get the percentage of completed Tasks in the Project. |

## Methods

| Name               | Description                               |
|--------------------|-------------------------------------------|
| Project(string)    | Create a new Project with the given name. |
| AddTask(Task)      | Add a Task to the Project.                |

_____________________________________________________________

# TaskCollection Class

A collection of TaskLists, maintain counts of total and completed Tasks.

## Definition

```
public class TaskCollection
```

- Namespace  
    - TaskManagements.Models

## Fields

| Name                      | Description                                         |
|---------------------------|-----------------------------------------------------|
| TotalTasksCount (int)         | The total number of Tasks in all TaskLists.           |
| IncompleteTasksCount (int)    | The number of incomplete Tasks in all TaskLists.      |

## Methods

| Name                              | Description                                         |
|-----------------------------------|-----------------------------------------------------|
| AddTaskList(TaskList)             | Add a TaskList to the TaskCollection.               |
| ClearAllCompletedTasks()          | Remove all completed Tasks from all TaskLists.      |
| Save(string)                      | Save the TaskCollection to a file.                  |
| Load(string)                      | Load the TaskCollection from a file.                |
| PrintAllTaskListTasks()           | Print all Tasks from all TaskLists.                 |
| ToString()                        | Return a text representation of the TaskCollection. |
| GetTasksSortedByName()            | Get all Tasks sorted by name.                       |
| GetTasksSortedByDate()            | Get all Tasks sorted by due date.                   |
| GetTasksSortedByCreationDate()    | Get all Tasks sorted by creation date.              |
| GetTasksSortedByPriority()        | Get all Tasks sorted by priority.                   |
| GetHabits()                       | Get all Tasks that are Habits.                      |
| GetRepeatingTasks()               | Get all Tasks that are RepeatingTasks.              |
| GetDueTasks()                     | Get all Tasks that are due.                         |
| GetTasksWithDescription(string)   | Get all Tasks with the given description.           |
| GetAllTasks()                     | Get all Tasks in the TaskCollection.                |

_____________________________________________________________

# StringParse Class

Static helpers for parsing friendly format Task creation strings.

## Definition

```
public static class StringParse
```

- Namespace  
    - TaskManagement.Helpers

## Methods

| Name                             | Description                                                          |
|----------------------------------|----------------------------------------------------------------------|
| ParseNaturalDate(string)         | Get a date from a natural langauge string.                           |
| ParseNaturalTime(string)         | Get a time of day from a natural language string.                    |
| ParsenaturalTaskCreation(string) | Get a Task with an optional due date from a natural langauge string. |