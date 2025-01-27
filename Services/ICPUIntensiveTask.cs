namespace PerformanceIssues.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private readonly List<byte[]> _buffers = new List<byte[]>(); 
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _disposed = false;

        public void Start()
        {
            // Logic to start the task
        }

        public void Stop()
        {
            // Logic to stop the task, including canceling the token
            _cancellationTokenSource.Cancel();
        }

        private void ClearResources()
        {
            // Clear large collections and other potential memory leaks
            _buffers.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    ClearResources();
                    _cancellationTokenSource.Dispose();
                }

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