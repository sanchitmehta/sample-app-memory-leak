using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed;

        public async Task GenerateAndStoreData(int count, CancellationToken cancellationToken)
        {
            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    Data = new byte[1024]  // 1KB of data per record
                };

                _random.NextBytes(data.Data);
                _storedData.Add(data);

                if (i % 1000 == 0)
                {
                    await Task.Delay(1, cancellationToken);
                }
            }
        }

        public void ClearStoredData()
        {
            _storedData.Clear();
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _storedData.Clear();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}