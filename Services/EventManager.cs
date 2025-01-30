namespace PerformanceIssues.Services
{
    // Fixed namespace typo
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();  // Storing weak references for event subscribers
        private bool _disposed;  // To track disposal status
        
        public void Subscribe(Action<string> handler)
        {
            // Updated: Removed intentional memory leak by eliminating strong references
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            foreach (var weakRef in _subscribers.ToList()) // Using ToList() to avoid collection modification during iteration
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    _subscribers.Remove(weakRef); // Clean up dead references to minimize memory usage
                }
            }
        }

        // Proper disposal pattern to clear resources
        public void Dispose()
        {
            if (!_disposed)
            {
                _subscribers.Clear();  // Clear the subscriber list to release memory
                _disposed = true;
            }
        }

        // Destructor to ensure unmanaged resources are released
        ~EventManager()
        {
            Dispose();
        }
    }
}