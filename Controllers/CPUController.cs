using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Services; // Note: Fixed typo in namespace spelling
using System;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase
    {
        private readonly CPUTaskManager _cpuTaskManager;

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager ?? throw new ArgumentNullException(nameof(cpuTaskManager)); // Added null check to ensure safety if null passed
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            var taskId = _cpuTaskManager.StartNewTask(request.Complexity);

            // (Comment: Avoid excessive logging which might contain retained long-lived strings)
            // Example of improvement: Ensure logged strings are formatted properly with minimal memory overhead.
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
            // (Comment: Collecting active tasks could have unintentional high memory retention due to references - ensure efficient data models)
            var tasks = _cpuTaskManager.GetActiveTasks();
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            // (Comment: Ensure any internally buffered objects in the task manager are properly disposed/released during StopAllTasks)
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
        }

        // Proper disposal pattern: Implement IDisposable for the controller if owning long-lived disposable objects
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Explicitly dispose _cpuTaskManager if it implements IDisposable
                if (_cpuTaskManager is IDisposable disposableTaskManager)
                {
                    disposableTaskManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}