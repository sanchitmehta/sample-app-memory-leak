namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference<Action<string>>> _subscribers = new();
        private bool _disposed;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            // Add a weak reference for the subscriber
            _subscribers.Add(new WeakReference<Action<string>>(handler));
        }

        public void RaiseEvent(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager));

            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.TryGetTarget(out var handler))
                {
                    handler?.Invoke(message);
                }
                else
                {
                    // Remove dead references to avoid unnecessary growth
                    _subscribers.Remove(weakRef);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _subscribers.Clear(); // Release all held references
                _disposed = true;
                GC.SuppressFinalize(this); // Prevent finalizer from running
            }
        }

        ~EventManager()
        {
            Dispose();
        }
    }
}