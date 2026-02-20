using System;



namespace OOP_Part1.Models
{
internal class Task
{
    public string              Description;
    public string              Notes = "";
    public bool                IsComplete = false;
    public readonly DateTime   DateCreated;


    Task(string description)
    {
        Description = description; 
        DateCreated = DateTime.Now;
    }

    public void ToggleCompletion()
    {
        IsComplete = !IsComplete;
    }
}
}
