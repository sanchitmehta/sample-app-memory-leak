namespace PerformanceIssues.Models
{
    using System;
    using System.Net.Http;
    using System.Text;

    public class CacheEntryRequest : IDisposable
    {
        private bool disposed;

        public int SizeMB { get; set; }

        // Properly implement the Dispose pattern with finalizer
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Release managed resources if any
                }
                // Release unmanaged resources if applicable
                disposed = true;
            }
        }

        ~CacheEntryRequest()
        {
            Dispose(false);
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        private bool disposed;

        public int Complexity { get; set; }

        // Implement Dispose pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Release managed resources if any
                }
                // Release unmanaged resources if applicable
                disposed = true;
            }
        }

        ~CPUTaskRequest()
        {
            Dispose(false);
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        private byte[] largeByteArray;
        private HttpClient httpClient;
        private bool disposed;

        public int RecordCount { get; set; }

        public DataGenerationRequest()
        {
            // Initialize disposable objects
            largeByteArray = new byte[1024 * 1024 * 10]; // Example: 10MB allocated
            httpClient = new HttpClient();
        }

        public void GenerateData()
        {
            try
            {
                // Simulate data generation
                for (int i = 0; i < RecordCount; i++)
                {
                    largeByteArray[i % largeByteArray.Length] = 0xFF;
                }
            }
            catch (Exception)
            {
                // Log exception if needed
                throw;
            }
        }

        public void ClearResources()
        {
            // Clear large byte array once done to prevent memory leaks
            if (largeByteArray != null)
            {
                Array.Clear(largeByteArray, 0, largeByteArray.Length);
                largeByteArray = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    ClearResources();

                    if (httpClient != null)
                    {
                        httpClient.Dispose();
                        httpClient = null;
                    }
                }
                // Release unmanaged resources if applicable
                disposed = true;
            }
        }

        ~DataGenerationRequest()
        {
            Dispose(false);
        }
    }
}