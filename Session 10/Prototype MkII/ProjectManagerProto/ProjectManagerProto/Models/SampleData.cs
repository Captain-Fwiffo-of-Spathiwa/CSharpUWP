using System;

namespace ProjectManagerProto.Models
{
    public static class SampleData
    {
        public static TaskCollection Create()
        {
            var collection = new TaskCollection();

            var project1 = new Project("Website Redesign");
            project1.AddTask(new Task("Design new homepage") { DueDate = DateTime.Now.AddDays(3) });
            project1.AddTask(new Task("Update logo") { IsComplete = true });
            project1.AddTask(new Task("Review content"));

            var project2 = new Project("Dog Walking Business");
            project2.AddTask(new Task("Write launch blog post"));
            project2.AddTask(new Task("Test doggos") { IsComplete = true });
            project2.AddTask(new Task("Test puppers") { IsComplete = true });
            project2.AddTask(new Task("Get more doggos"));

            collection.AddTaskList(project1);
            collection.AddTaskList(project2);

            return collection;
        }
    }
}