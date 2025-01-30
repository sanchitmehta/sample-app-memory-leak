namespace PerformanceIssues.Services
{
    // Interface should not have any resource management responsibility. 
    // Ensure that any implementation of this interface follows proper 
    // disposal patterns where necessary.
    public interface ICPUIntensiveTask
    {
        // Start method, ensure any resources allocated in the implementation are properly managed.
        void Start(); 

        // Stop method, ensure that resources in implementations are released.
        void Stop(); 
    }
}