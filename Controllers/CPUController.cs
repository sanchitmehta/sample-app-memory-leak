using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Serivces;
using System;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase
    {
        private readonly CPUTaskManager _cpuTaskManager;

        // Constructor dependency injection of CPUTaskManager
        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager ?? throw new ArgumentNullException(nameof(cpuTaskManager));
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            // Ensure proper cleanup of temporary data (if necessary at the TaskManager level)
            var taskId = _cpuTaskManager.StartNewTask(request.Complexity);
            return Ok(new { taskId });
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            // Validate inputs to avoid unnecessary processing
            if (string.IsNullOrWhiteSpace(taskId))
                return BadRequest("TaskId cannot be null or empty");

            // Ensure task stopping is performed without leaking unmanaged resources or growing memory
            if (!_cpuTaskManager.StopTask(taskId))
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            // Fetch active tasks, ensure no excessive allocations or copies of data
            var tasks = _cpuTaskManager.GetActiveTasks();

            // If tasks list is large, consider optimization at service level to return paged data or metadata
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            // Stop all tasks carefully disposing resources
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
        }

        // Override the Dispose method to ensure proper disposal of services
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cleanup the injected CPUTaskManager, if it implements IDisposable
                if (_cpuTaskManager is IDisposable disposableTaskManager)
                {
                    disposableTaskManager.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}