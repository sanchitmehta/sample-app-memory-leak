namespace PerformanceIssues.Serivces
{
    // Refactored to address potential memory leaks while retaining the intended functionality
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference<Action<string>>> _subscribers = new(); // Use generic WeakReference for clarity
        private bool _disposed = false; // To track resource cleanup

        // Subscribe method adds handlers weakly to avoid unnecessary memory retention
        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _subscribers.Add(new WeakReference<Action<string>>(handler));
        }

        // Raises event for all valid (non-collected) subscribers
        public void RaiseEvent(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager));

            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.TryGetTarget(out var handler))
                {
                    handler(message);
                }
                else
                {
                    // Remove invalid references to avoid growth in list
                    _subscribers.Remove(weakRef);
                }
            }
        }

        // Ensures proper cleanup of any resources
        public void Dispose()
        {
            if (_disposed) return;

            // Cleanup logic (if necessary) goes here
            _subscribers.Clear(); // Clear all subscriptions to release references
            _disposed = true;
        }
    }
}