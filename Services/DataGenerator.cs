namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed;

        public async Task GenerateAndStoreData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    Data = new byte[1024]  // 1KB of data per record
                };

                try
                {
                    _random.NextBytes(data.Data);
                    _storedData.Add(data);  // Ensure data is cleared later to avoid memory leak
                }
                catch
                {
                    (data as IDisposable)?.Dispose();  // Dispose if the object implements IDisposable
                    throw;
                }

                if (i % 1000 == 0)
                {
                    await Task.Delay(1);  // Give other threads a chance to run
                }
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public void ClearStoredData()
        {
            _storedData.Clear();  // Explicitly clear the collection when no longer needed
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Clear resources
                ClearStoredData();
            }

            _disposed = true;
        }
    }
}