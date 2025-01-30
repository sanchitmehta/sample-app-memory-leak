using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Services;
using System;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase
    {
        private readonly CPUTaskManager _cpuTaskManager;
        private bool _disposed = false; // Flag for disposing and preventing double disposal

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager ?? throw new ArgumentNullException(nameof(cpuTaskManager));
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            var taskId = _cpuTaskManager.StartNewTask(request.Complexity);

            // Ensure the task manager is tracking tasks properly
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
            // Wrap the result in a using scope if necessary for large collections
            var tasks = _cpuTaskManager.GetActiveTasks();
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
        }

        // Ensure disposal of unmanaged resources
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose only if the controller owns the CPUTaskManager instance
                    if (_cpuTaskManager is IDisposable cpuTaskManagerDisposable)
                    {
                        cpuTaskManagerDisposable.Dispose();
                    }
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}