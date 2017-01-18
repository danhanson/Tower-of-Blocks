using System;
using System.ComponentModel;

public enum TaskStatus
{
    Inactive,
    Waiting,
    Computed
}

public abstract class Task<T>
{
    TaskStatus status;

    public TaskStatus Status
    {
        get { return status; }
    }

    public bool IsDone
    {
        get { return Status == TaskStatus.Computed; }
    }

    T value;
    Exception exception;

    public Task()
    {
        status = TaskStatus.Inactive;
    }

    public T Value
    {
        get
        {
            if (exception != null)
                throw exception;
            return value;
        }
    }

    public void DoWork(object sender, DoWorkEventArgs args)
    {
        status = TaskStatus.Waiting;
        try
        {
            value = Compute(sender, args);
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            status = TaskStatus.Computed;
        }
    }

    public abstract T Compute(object sender, DoWorkEventArgs args);
}
