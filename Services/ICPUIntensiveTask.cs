using System;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        // Suggestion: Use CancellationTokenSource carefully to prevent memory leaks.
        private CancellationTokenSource _cts;
        private Task _workerTask;

        // Consider lazy initialization or null check for efficient resource handling.
        private byte[] _cache; // Represents System.Byte[]

        // Constructor
        public CPUIntensiveTask()
        {
            _cts = new CancellationTokenSource();
            _cache = new byte[1024 * 1024]; // Simulate memory-intensive object. Dispose properly.
        }

        public void Start()
        {
            if (_cts == null)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            _workerTask = Task.Run(() => DoWork(_cts.Token), _cts.Token);
        }

        public void Stop()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel(); // Cancel the token to stop the task.
                _workerTask?.Wait(); // Ensure proper task completion.
            }
        }

        private void DoWork(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Simulated CPU-intensive work.
                // Suggestion: Avoid memory leaks with large strings by keeping control over their creation/update frequency.
                string tempData = new string('x', 1024 * 1024); // System.String usage.
                ProcessData(tempData);
            }
        }

        private void ProcessData(string data)
        {
            // Placeholder for actual processing logic.
            // Suggestion: Avoid holding large strings for longer than absolutely necessary.
        }

        public void Dispose()
        {
            // Ensure proper cleanup of CancellationTokenSource.
            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }

            // Set cache to null to allow garbage collection.
            _cache = null;

            // Wait and clean up the worker task.
            _workerTask?.Dispose();
        }
    }
}