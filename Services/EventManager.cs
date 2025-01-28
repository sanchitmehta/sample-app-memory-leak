namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
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
                else
                {
                    _subscribers.Remove(weakRef);
                }
            }
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
                    // Dispose managed resources
                    _subscribers.Clear();
                }

                // Dispose unmanaged resources if there were any

                _disposed = true;
            }
        }

        ~EventManager()
        {
            Dispose(false);
        }
    }
}