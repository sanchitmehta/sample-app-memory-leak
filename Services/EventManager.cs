namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference<Action<string>>> _subscribers = new();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
            _subscribers.Add(new WeakReference<Action<string>>(handler));
        }

        public void RaiseEvent(string message)
        {
            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.TryGetTarget(out Action<string> handler))
                {
                    handler(message);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _subscribers.Clear();
                _disposed = true;
            }
        }
    }
}