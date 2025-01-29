using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private CancellationTokenSource _cancellationTokenSource; // Fix: Manages task cancellation
        private Task _processingTask; // Fix: Maintain the task reference for proper cleanup

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (_cancellationTokenSource != null) 
                throw new InvalidOperationException("Task is already running.");

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            // Capture the processing task for proper disposal later
            _processingTask = Task.Run(() =>
            {
                try
                {
                    while (_isRunning && !token.IsCancellationRequested)
                    {
                        var results = new double[1000];

                        for (int i = 0; i < _complexity; i++)
                        {
                            results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                              Math.Sqrt(Math.Abs(Math.Tan(i)));
                        }

                        lock (_results) // Thread-safe handling of _results
                        {
                            // Improvement: Limit growth of _results to avoid memory leaks
                            if (_results.Count > 100)
                            {
                                _results.RemoveAt(0); // Remove oldest entries to control memory usage
                            }

                            _results.Add(results);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation happens, safely ignore
                }
                catch (Exception ex)
                {
                    // Log and handle unexpected errors if a logging mechanism exists
                    Console.WriteLine($"Unexpected exception: {ex.Message}");
                }
            }, token);
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;

            // Signal cancellation and wait for the task to stop gracefully
            _cancellationTokenSource?.Cancel();

            try
            {
                _processingTask?.Wait(); // Ensure any exceptions during task execution are surfaced
            }
            finally
            {
                CleanupTask();
            }
        }

        // Ensure proper cleanup of resources
        private void CleanupTask()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            _processingTask = null;
        }

        public void Dispose()
        {
            // Ensure resources are released when the object is disposed
            Stop();
        }
    }
}