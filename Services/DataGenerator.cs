namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

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
                    Data = GenerateRandomData(1024) // Ensure proper disposal where applicable
                };

                _storedData.Add(data);

                if (i % 1000 == 0)
                {
                    await Task.Delay(1);
                }
            }

            ClearStoredData(); // Prevent memory growth by clearing the collection once done
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private byte[] GenerateRandomData(int size)
        {
            var buffer = new byte[size];
            _random.NextBytes(buffer);
            return buffer;
        }

        public void ClearStoredData()
        {
            _storedData.Clear();
        }

        protected virtual void Dispose(bool disposing)
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
    }
}