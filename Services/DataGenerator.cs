namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<byte[]> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        public async Task GenerateAndStoreData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var data = new byte[1024];  // 1KB of data per record
                _random.NextBytes(data);
                _storedData.Add(data);  // Memory leak: storing without bounds

                if (i % 1000 == 0)
                {
                    await Task.Delay(1);  // Give other threads a chance to run
                }
            }

            ClearStoredData();  // Clear data after processing
        }

        private void ClearStoredData()
        {
            _storedData.Clear();
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
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
                ClearStoredData();  // Free managed resources
            }

            _disposed = true;
        }

        ~DataGenerator()
        {
            Dispose(false);
        }
    }
}