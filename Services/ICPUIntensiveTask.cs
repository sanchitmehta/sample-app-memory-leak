using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask
    {
        void Start();
        void Stop();
    }

    // Implementation of ICPUIntensiveTask
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly HttpClient _httpClient; // Ensure proper disposal of HttpClient
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private bool _disposed;

        public CPUIntensiveTask()
        {
            // HttpClient is instantiated once and reused to avoid socket exhaustion
            _httpClient = new HttpClient();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            // Ensure the previous task is stopped before starting a new one
            Stop();

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _task = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // Replace with actual CPU-intensive work
                        // Using HttpClient in a controlled way
                        using (var response = await _httpClient.GetAsync("https://example.com", token).ConfigureAwait(false))
                        {
                            response.EnsureSuccessStatusCode();
                            var data = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                            // Process data here
                            // Dispose unnecessary large byte arrays promptly
                            Array.Clear(data, 0, data.Length);
                        }

                        await Task.Delay(1000, token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // Task cancellation is expected, allow graceful exit
                    }
                    catch (Exception ex)
                    {
                        // Log exception (limit retention of logs if causing memory issues)
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }, token);
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();

                try
                {
                    _task?.Wait();
                }
                catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is OperationCanceledException))
                {
                    // Expected during normal cancellation
                }
                finally
                {
                    // Clean up cancellation token and task properly
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                    _task = null;
                }
            }
        }

        public void Dispose()
        {
            // Avoid double-dispose issues
            if (_disposed)
                return;

            _disposed = true;

            // Clean up resources
            Stop();
            _httpClient?.Dispose(); // Dispose of HttpClient
        }
    }
}