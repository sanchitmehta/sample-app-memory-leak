namespace PerformanceIssues.Serivces
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly List<Action<string>> _strongSubscribers = new();  

        private bool _disposed; // Flag to track disposal state

        public void Subscribe(Action<string> handler)
        {
            // Fix: Removed the strong reference to avoid memory leaks
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
            }
        }

        // Dispose method implementation for proper cleanup
        public void Dispose()
        {
            if (!_disposed)
            {
                // Clear the list to release memory
                _subscribers.Clear();
                
                // Mark as disposed to avoid repeated disposal
                _disposed = true;
            }
        }
    }
}