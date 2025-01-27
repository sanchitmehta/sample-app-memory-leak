namespace PerformanceIssues.Services
{
    using System;
    using System.Threading;
    using System.Collections.Generic;

    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;
        private List<byte[]> _largeByteArrayCache;

        public CPUIntensiveTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _largeByteArrayCache = new List<byte[]>();
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            // Start logic for CPU-intensive task
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            _cancellationTokenSource?.Cancel();

            ClearLargeCollections();
        }

        private void ClearLargeCollections()
        {
            if (_largeByteArrayCache != null)
            {
                _largeByteArrayCache.Clear();
                _largeByteArrayCache = null;
            }
        }

        private void ClearEventHandlers()
        {
            // Assume there are events, clearing them here
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearEventHandlers();

                    // Dispose managed resources
                    _cancellationTokenSource?.Dispose();
                    ClearLargeCollections();
                }

                // Free unmanaged resources if any

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~CPUIntensiveTask()
        {
            Dispose(disposing: false);
        }
    }
}