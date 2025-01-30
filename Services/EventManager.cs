namespace PerformanceIssues.Services
{
    // Updated to address memory leaks in EventManager class
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();

        // Removed strong reference list to prevent memory leaks
        // private readonly List<Action<string>> _strongSubscribers = new(); 

        private bool _isDisposed = false; // Added for proper disposal tracking

        public void Subscribe(Action<string> handler)
        {
            // Storing only weak references to allow garbage collection
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

        // Proper cleanup and disposal of resources
        public void Dispose()
        {
            if (_isDisposed)
                return;

            // Dispose pattern to release resources
            _subscribers.Clear(); // Clears weak references to prevent holding on to memory
            _isDisposed = true;

            // Call GC.SuppressFinalize to avoid redundant finalization
            GC.SuppressFinalize(this);
        }

        ~EventManager()
        {
            // Finalizer in case Dispose was not explicitly called
            Dispose();
        }
    }
}