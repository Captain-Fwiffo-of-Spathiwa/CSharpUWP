using System;

namespace ProjectManagementProto.Models
{
    public static class SampleData
    {
        public static TaskCollection Create()
        {
            var collection = new TaskCollection();

            var project1 = new Project("Website Redesign");
            project1.AddTask(new Task("Design homepage") { DueDate = DateTime.Now.AddDays(3) });
            project1.AddTask(new Task("Implement login") { IsComplete = true });

            var project2 = new Project("Mobile App");
            project2.AddTask(new Task("Setup project structure"));
            project2.AddTask(new Task("Create onboarding screens") { DueDate = DateTime.Now.AddDays(7) });

            collection.AddTaskList(project1);
            collection.AddTaskList(project2);

            return collection;
        }
    }
}