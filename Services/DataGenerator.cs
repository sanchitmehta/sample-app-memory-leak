namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        // generates about 1MB of data per minute
        public async Task GenerateAndStoreData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Properly structure the disposable 'data.Data' (byte[] objects).
                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    Data = new byte[1024]  // ~1 KB
                };

                try
                {
                    _random.NextBytes(data.Data); // Fill byte[] efficiently
                    _storedData.Add(data);  // Ensure _storedData cleanup in Dispose()
                }
                catch
                {
                    // Log or track failed operations here (if needed).
                    throw;
                }

                // ~1 ms delay => 1024 ms for 1024 records => ~1 MB/s
                await Task.Delay(1);
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(
                Enumerable
                    .Repeat(chars, length)
                    .Select(s => s[_random.Next(s.Length)])
                    .ToArray()
            );
        }

        // Implement IDisposable to perform cleanup of unmanaged resources and large objects
        public void Dispose()
        {
            if (!_disposed)
            {
                // Explicitly clear stored data to release memory
                _storedData.Clear();

                // Note: No explicit disposal for byte[] (automatically garbage-collected)
                // However, if _storedData contained other IDisposable objects, we would dispose them here.

                _disposed = true;

                // Suppress finalization to avoid GC overhead
                GC.SuppressFinalize(this);
            }
        }
    }
}