using System.ComponentModel;
using System.Runtime.CompilerServices;
using TaskManagement.Models;
using ModelTask = TaskManagement.Models.Task;

namespace ProjectManagerProto.ViewModels;

public sealed class TaskViewModel : INotifyPropertyChanged
{
    private readonly ModelTask _task;

    public string Description
    {
        get => _task.GetDescription();
        set
        {
            _task.SetDescription(value);
            OnPropertyChanged();
        }
    }

    public string Notes
    {
        get => _task.Notes;
        set
        {
            _task.Notes = value;
            OnPropertyChanged();
        }
    }

    public int PriorityValue
    {
        get => _task.TaskPriority.Value;
        set
        {
            _task.TaskPriority = new Priority(value);
            OnPropertyChanged();
        }
    }

    public DateTime DueDate
    {
        get => _task.DueDate.Value;
        set
        {
            _task.DueDate = value;
            OnPropertyChanged();
        }
    }

    public bool IsComplete
    {
        get => _task.IsComplete;
        set
        {
            _task.IsComplete = value;
            OnPropertyChanged();
        }
    }

    public DateTime DateCreated => _task.DateCreated;

    public TaskViewModel(ModelTask task)
    {
        _task = task;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}