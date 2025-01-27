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
    private readonly List<Action<string>> _handlers = new();
    private bool _disposed;

    public void Subscribe(Action<string> handler)
    {
        _handlers.Add(handler);
    }

    public void RaiseEvent(string message)
    {
        foreach (var handler in _handlers)
        {
            handler?.Invoke(message);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        // Clear handlers to avoid memory leaks
        _handlers.Clear();
        _disposed = true;
    }
}

public class ServiceUsingEventManager
{
    private readonly IEventManager _eventManager;
    public ServiceUsingEventManager(IEventManager eventManager)
    {
        _eventManager = eventManager;
    }

    public void DoWork()
    {
        _eventManager.Subscribe(OnEvent);
        _eventManager.RaiseEvent("Hello World");
    }

    private void OnEvent(string message)
    {
        Console.WriteLine(message);
    }
    
    public void Dispose()
    {
        if (_eventManager is IDisposable disposableEventManager)
        {
            disposableEventManager.Dispose();
        }
    }
}