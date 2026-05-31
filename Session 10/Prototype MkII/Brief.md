Brief – Project Management App
==============================

A project-based task management app. This is intended for people who have a lot of well organised
projects with sub-steps. It focuses on the priority of tasks and projects.

Purpose
-------

The Project Management App allows the user to manage multiple projects, with options to create, edit,
view, and delete projects.

Main Goals
----------

The main goals of the Project Management App are:
 - To be able to see all currently managed Projects in one place
 - To be able to add, remove, and edit Tasks within any given project
 - To be able to organise how Projects and Tasks are viewed

Arrangement
-----------

The app’s overarching layout is comprised of:
1. The Projects Home Window
2. A TaskList Window per Project
3. A Task Dialog per Task

PROJECTS HOME WINDOW
--------------------

The Projects Home Window contains 2 sections: the Projects list, and Settings. This window is where the
user can view all Projects, add new ones, and delete Projects manually and by completion.

The Settings section is on the left and contains 4 parts:
1. A button to add a new Project (cyan button)
   * Displays a modal dialog for the new Project’s name
2. Radio buttons to order projects by name, task count, or completion percentage
3. Radio buttons to show only incomplete projects, only complete projects, or all projects
4. A button to delete all completed Projects (dark red button)
   * Displays a warning dialog before deletion

The Projects List section is on the right and contains the list of Projects the app has saved.
* Each Project shows: Project name, Total Task Count, Completion Percentage, and Date Created
* Each Project shows a delete button
  * Displays a modal dialog to confirm
* Each Project can be double-clicked
  * Displays the clicked Project in a new window; the TaskList window 
* If that Project’s window is already open, double-clicking instead shifts focus to that window

TASKLIST WINDOW
---------------

The TaskList Window contains 2 sections: the TaskList, and Settings. This window is where the user can
view all Tasks in a Project, add new Tasks, and delete Tasks manually and by completion.

The Settings section is on the left and contains 4 parts:
1. A button to add a new Task (green button)
   * Displays a modal dialog with a text field for the new Task (see Notes)
2. Radio buttons to order Projects by name, due date, or priority
3. Radio buttons to show only incomplete Tasks, complete Tasks, overdue Tasks, or all Tasks
4. A button to delete all completed Tasks (dark red button)
   * Display a warning dialog before deletion

The TaskList section is on the right and contains the list of Tasks in the current Project.
* Each Task shows: Name, Completion Status, Date Created, and Due Date
* Each Task shows a delete button
   * Displays a modal dialog to confirm
* Each Task can be double-clicked
   * Display the Task in a modal dialog; the Task dialog

TASK DIALOG
-----------

The Task Dialog contains Task fields and OK/Cancel buttons only. This is a modal dialog where the user
can view and edit details of a Task, and set its completed status.

* Editable fields are:
  * Description
  * Notes
  * Priority
  * Due date
  * Completion status
* The non-editable field is:
  * Date created

Notes:
------

* Saving to disk not required in prototype
* When adding a new Task, the friendly format pattern is used, allowing entry in natural text
  * Friendly formatting not required in prototype
* Changes to a Task or Tasklist are updated automatically in the Projects Home Window
* Closing the Projects Home Screen will close all TaskList windows.