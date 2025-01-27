namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }
}

public class EventManager : IEventManager
{
    private event Action<string> _event;
    private bool _disposed = false;
    
    public void Subscribe(Action<string> handler)
    {
        _event += handler;
    }

    public void RaiseEvent(string message)
    {
        _event?.Invoke(message);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Clear event handlers
                if (_event != null)
                {
                    foreach (Delegate d in _event.GetInvocationList())
                    {
                        _event -= (Action<string>)d;
                    }
                }
            }
            _disposed = true;
        }
    }

    ~EventManager()
    {
        Dispose(false);
    }
}

public class EventManagerUsage
{
    public void UseEventManager()
    {
        using (var eventManager = new EventManager())
        {
            eventManager.Subscribe(OnEventRaised);
            eventManager.RaiseEvent("Test event");
        }
        // eventManager is disposed here automatically
    }

    private void OnEventRaised(string message)
    {
        Console.WriteLine(message);
    }
}