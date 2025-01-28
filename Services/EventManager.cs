
namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly List<Action<string>> _temporarySubscribers = new();
        private bool _disposed = false;
        
        public void SubscribeWeak(Action<string> handler)
        {
            _subscribers.Add(new WeakReference(handler));
        }

        public void SubscribeStrong(Action<string> handler)
        {
            _temporarySubscribers.Add(handler);
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

            foreach (var handler in _temporarySubscribers)
            {
                handler(message);
            }
        }

        public void ClearStrongSubscribers()
        {
            _temporarySubscribers.Clear();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _subscribers.Clear();
                _temporarySubscribers.Clear();
                _disposed = true;
            }
        }

        ~EventManager()
        {
            Dispose();
        }
    }
}