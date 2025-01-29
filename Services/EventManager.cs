namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private bool _isDisposed = false;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(EventManager));

            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    _subscribers.Remove(weakRef); // Cleanup stale weak references
                }
            }
        }

        public void ClearSubscribers()
        {
            _subscribers.Clear(); // Clear the collection when it's no longer needed
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _subscribers.Clear();
                _isDisposed = true;
            }
        }
    }
}