using System;
using System.Windows.Input;
using ProjectManagerProto.Models;
using Microsoft.Maui.Controls;
using Task = ProjectManagerProto.Models.Task;

namespace ProjectManagerProto.ViewModels
{
    public class TaskDialogViewModel
    {
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        public string Notes { get; set; }
        public bool IsComplete { get; set; }
        public DateTime DateCreated { get; }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private readonly INavigation _navigation;
        private readonly Task _task;

        public bool IsOk { get; private set; }

        public TaskDialogViewModel(Task task, INavigation navigation)
        {
            _task = task;
            _navigation = navigation;

            Description = task.Description;
            DueDate = task.DueDate;
            Priority = task.TaskPriority.Value;
            Notes = task.Notes;
            IsComplete = task.IsComplete;
            DateCreated = task.DateCreated;

            OkCommand = new Command(async () =>
            {
                task.Description = Description;
                task.DueDate = DueDate;
                task.TaskPriority = new Priority(Priority);
                task.Notes = Notes;
                task.IsComplete = IsComplete;
                IsOk = true;
                await _navigation.PopModalAsync();
            });

            CancelCommand = new Command(async () =>
            {
                IsOk = false;
                await _navigation.PopModalAsync();
            });
        }
    }
}