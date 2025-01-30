using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private bool _disposed = false; // Flag to detect redundant calls to Dispose

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            _isRunning = true;
            Task.Run(() =>
            {
                while (_isRunning)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }

                    lock (_results) // Introduced lock to ensure thread-safety
                    {
                        if (_results.Count >= 100) // Limit the size of the results list to prevent unbounded growth
                        {
                            _results.RemoveAt(0); // Remove oldest entry to conserve memory
                        }
                        _results.Add(results);
                    }
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
        }

        // Proper implementation of IDisposable to clear resources
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization to avoid double cleanup
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _isRunning = false; // Ensure the task has stopped
                lock (_results) 
                {
                    _results.Clear(); // Free up memory by clearing the list
                }
            }

            _disposed = true;
        }

        ~CPUIntensiveTask()
        {
            Dispose(false);
        }
    }
}