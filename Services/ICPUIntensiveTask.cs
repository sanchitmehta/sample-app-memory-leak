namespace PerformanceIssues.Services
{
    using System;
    using System.IO;
    using System.Threading;

    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private Timer _timer;
        private MemoryStream _memoryStream;
        private bool _disposed;

        public CPUIntensiveTask()
        {
            _memoryStream = new MemoryStream();
        }

        public void Start()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CPUIntensiveTask));
            _timer = new Timer(PerformTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CPUIntensiveTask));
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void PerformTask(object state)
        {
            if (_disposed) return;

            // Simulating CPU intensive task
            byte[] buffer = new byte[1024];
            _memoryStream.Write(buffer, 0, buffer.Length);

            // Clear the stream periodically to free up memory
            if (_memoryStream.Length > 1024 * 1024)
            {
                _memoryStream.SetLength(0);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _timer?.Dispose();
            _memoryStream?.Dispose();
            _disposed = true;
        }
    }
}