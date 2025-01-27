using System;
using System.Collections.Generic;
using System.Threading;
using System.IO.Pipelines;
using System.Net.Http;

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }

        public void Dispose()
        {
            // Add disposal logic if there's any unmanaged resource.
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }

        public void Dispose()
        {
            // Add disposal logic if there's any unmanaged resource.
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        public void Dispose()
        {
            // Add disposal logic if there's any unmanaged resource.
        }
    }

    public class HttpClientWrapper : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public HttpResponseMessage SendRequest(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request).Result;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    public class Logger : IDisposable
    {
        private readonly LoggerFactoryScopeProvider _loggerFactoryScopeProvider = new LoggerFactoryScopeProvider();

        public void Log(string message)
        {
            using (var scope = _loggerFactoryScopeProvider.CreateScope())
            {
                // Logging logic here
            }
        }

        public void Dispose()
        {
            _loggerFactoryScopeProvider?.Dispose();
        }
    }

    public class CancellationTokenSourceWrapper : IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public CancellationToken Token => _cts.Token;

        public void Cancel()
        {
            _cts.Cancel();
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }

    public class PipeManager : IDisposable
    {
        private readonly Pipe _pipe = new Pipe();

        public PipeReader Reader => _pipe.Reader;
        public PipeWriter Writer => _pipe.Writer;

        public void Dispose()
        {
            _pipe?.Reset();
        }
    }

    public class NetworkManager : IDisposable
    {
        private List<IPAddress> _ipAddresses = new List<IPAddress>();
        private List<IPEndPoint> _endPoints = new List<IPEndPoint>();

        public void AddIPAddress(IPAddress ipAddress)
        {
            _ipAddresses.Add(ipAddress);
        }

        public void AddEndPoint(IPEndPoint endPoint)
        {
            _endPoints.Add(endPoint);
        }

        public void Dispose()
        {
            _ipAddresses?.Clear();
            _endPoints?.Clear();
        }
    }
}