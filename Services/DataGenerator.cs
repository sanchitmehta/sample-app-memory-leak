namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData;
        private readonly Random _random;
        private bool _disposed;

        public DataGenerator()
        {
            _storedData = new List<object>();
            _random = new Random();
            _disposed = false;
        }

        public async Task GenerateAndStoreData(int count, CancellationToken cancellationToken)
        {
            for (int i = 0; i < count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    Data = new byte[1024]
                };

                _random.NextBytes(data.Data);
                _storedData.Add(data);

                if (i % 1000 == 0)
                {
                    await Task.Delay(1, cancellationToken);
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
            _storedData.Clear();
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
                    ClearStoredData();
                }

                _disposed = true;
            }
        }
    }
}