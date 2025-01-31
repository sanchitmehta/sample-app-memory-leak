namespace PerformanceIssues.Services
{
    // Updated interface to include IDisposable to ensure proper disposal of resources if necessary.
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();

        // Suggested improvement (comment): Since this involves CPU-intensive tasks, ensure that any unmanaged resources or worker threads are stopped and cleaned up properly during disposal.
    }
}

