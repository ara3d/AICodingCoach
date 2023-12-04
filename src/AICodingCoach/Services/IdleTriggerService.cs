namespace AICodingCoach.Services;

/// <summary>
/// Triggers an action when nothing happens for a period of time
/// after the last time the "Dirty" flag is set.
/// </summary>
public class IdleTriggerService
{
    private Thread IdleMonitoringThread;

    public void OnIdleThreadStart(object? _)
    {
        while (true)
        {
            if (IsDirty && IdleTimeSpan.TotalSeconds > IdleSeconds)
            {
                IsDirty = false;
                OnIdleAction();
            }

            Thread.Sleep((int)(IdleSeconds * 500));
        }
        // ReSharper disable once FunctionNeverReturns
    }

    public readonly double IdleSeconds;
    public readonly Action OnIdleAction;

    public IdleTriggerService(Action action, double idleSeconds)
    {
        IdleMonitoringThread = new Thread(OnIdleThreadStart);
        OnIdleAction = action;
        IdleSeconds = idleSeconds;
        IdleMonitoringThread.Start();
    }

    private bool _isDirty = false;
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (!value)
            {
                _isDirty = false;
            }
            else 
            {
                _isDirty = true;
                SetDirtyWhen = DateTimeOffset.Now;
            }
        }
    }

    public DateTimeOffset SetDirtyWhen = DateTimeOffset.MinValue;
    public TimeSpan IdleTimeSpan => DateTimeOffset.Now - SetDirtyWhen;
}