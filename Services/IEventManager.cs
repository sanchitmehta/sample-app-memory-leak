namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager
    {
        private readonly List<Action<string>> _subscribers = new List<Action<string>>();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _subscribers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            foreach (var subscriber in _subscribers.ToList())
            {
                try
                {
                    subscriber(message);
                }
                catch
                {
                    // Optionally log or handle subscriber exceptions
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _subscribers.Clear();
            _disposed = true;
        }
    }
}