namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        public async Task GenerateAndStoreData(int count)
        {
            try
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

                    _random.NextBytes(data.Data);

                    if (_storedData.Count >= 10000)
                    {
                        ClearStoredData(); // Avoid unbounded growth of the collection
                    }

                    _storedData.Add(data);

                    if (i % 1000 == 0)
                    {
                        await Task.Delay(1);  // Give other threads a chance to run
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions as needed
                throw;
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private void ClearStoredData()
        {
            _storedData.Clear();
            GC.Collect(); // Trigger garbage collection to release memory
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearStoredData();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DataGenerator()
        {
            Dispose(false);
        }
    }
}