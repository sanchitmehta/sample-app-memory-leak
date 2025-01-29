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
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                            Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }

                    lock (_results)
                    {
                        if (_isRunning) // Ensure task is still running before adding results
                        {
                            _results.Add(results);
                        }
                    }
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;

            lock (_results)
            {
                _results.Clear(); // Clear collection to release memory
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
                    Stop(); // Stop the task and release resources
                }

                _disposed = true;
            }
        }
    }
}