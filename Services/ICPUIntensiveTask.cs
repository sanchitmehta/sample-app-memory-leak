using System;
using System.Net;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }
}

namespace PerformanceIssues.Services.Impl
{
    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Pipe _pipe;
        private Task _task;

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _pipe = new Pipe();
            
            _task = Task.Run(() => PerformTask(_cancellationTokenSource.Token));
        }

        private void PerformTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Simulate CPU intensive work
                byte[] data = new byte[1024];
                string content = Convert.ToBase64String(data);

                // Process data and perform necessary tasks
                _pipe.Writer.WriteAsync(data, cancellationToken).GetAwaiter().GetResult();
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource.Dispose();
            _pipe?.Reset();
        }
    }
}

namespace PerformanceIssues.Network
{
    public class NetworkResources : IDisposable
    {
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;

        public void AllocateResources()
        {
            _ipAddress = IPAddress.Parse("127.0.0.1");
            _ipEndPoint = new IPEndPoint(_ipAddress, 8080);
        }

        public void Dispose()
        {
            // Properly dispose of IPAddress and IPEndPoint if necessary
            _ipAddress = null;
            _ipEndPoint = null;
        }
    }
}

namespace PerformanceIssues.Managers
{
    public class TaskManager : IDisposable
    {
        private ICPUIntensiveTask _cpuIntensiveTask;

        public void InitializeTask()
        {
            _cpuIntensiveTask = new CPUIntensiveTask();
            _cpuIntensiveTask.Start();
        }

        public void TerminateTask()
        {
            _cpuIntensiveTask.Stop();
        }

        public void Dispose()
        {
            _cpuIntensiveTask?.Dispose();
        }
    }
}