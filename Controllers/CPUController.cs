using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Services;
using System.Linq;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase, IDisposable
    {
        private readonly CPUTaskManager _cpuTaskManager;
        private bool _disposed = false;

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager;
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            var taskId = _cpuTaskManager.StartNewTask(request.Complexity);
            return Ok(new { taskId });
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            if (!_cpuTaskManager.StopTask(taskId))
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            var tasks = _cpuTaskManager.GetActiveTasks();
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _cpuTaskManager.Dispose();
                }

                // Dispose unmanaged resources here if any
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CPUController()
        {
            Dispose(false);
        }
    }
}

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private bool _disposed = false;

        public string StartNewTask(int complexity)
        {
            // Implementation for starting new task
            return Guid.NewGuid().ToString();
        }

        public bool StopTask(string taskId)
        {
            // Implementation for stopping task
            return true;
        }

        public List<string> GetActiveTasks()
        {
            // Implementation for retrieving active tasks
            return new List<string>();
        }

        public void StopAllTasks()
        {
            // Implementation for stopping all tasks
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources and event handlers
                }
                // Dispose unmanaged resources here if any
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CPUTaskManager()
        {
            Dispose(false);
        }
    }
}