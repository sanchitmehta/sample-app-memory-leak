using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private bool _disposed;

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
                    // Avoid unbounded growth by introducing a limit
                    if (_results.Count > 100)
                    {
                        // Remove oldest batches to prevent memory leak
                        _results.RemoveAt(0);
                    }

                    var results = new double[1000];
                    for (int i = 0; i < Math.Min(_complexity, 1000); i++) // Prevent array overflows
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }

                    // Store bounded results
                    _results.Add(results);
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;

            // Clear results after stopping to release memory
            _results.Clear();
        }

        // Implement IDisposable to ensure proper resource cleanup
        public void Dispose()
        {
            // Check if already disposed
            if (_disposed) return;

            // Stop task and clear results
            Stop();

            // Perform any additional cleanup if required

            _disposed = true;

            // Suppress finalization
            GC.SuppressFinalize(this);
        }
    }
}