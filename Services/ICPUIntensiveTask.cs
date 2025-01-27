namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private bool _disposed = false;

        // Example of a large collection that needs to be cleared properly
        private List<int> _largeCollection;

        // Example of an EventHandler that needs to be handled properly
        public event EventHandler TaskCompleted;

        public CPUIntensiveTask()
        {
            _largeCollection = new List<int>();
        }

        public void Start()
        {
            // Start the CPU intensive task
        }

        public void Stop()
        {
            // Stop the CPU intensive task
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                    if (_largeCollection != null)
                    {
                        _largeCollection.Clear();
                        _largeCollection = null;
                    }

                    // Clear event handlers
                    if (TaskCompleted != null)
                    {
                        foreach (EventHandler d in TaskCompleted.GetInvocationList())
                        {
                            TaskCompleted -= d;
                        }
                    }
                }

                // Dispose unmanaged resources here

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CPUIntensiveTask()
        {
            Dispose(false);
        }
    }
}