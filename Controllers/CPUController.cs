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

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            // Ensure proper disposal pattern for dependency injection; no disposal needed here as DI container handles lifecycle.
            _cpuTaskManager = cpuTaskManager;
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            // StartNewTask() may allocate resources; ensure task manager disposes objects correctly internally.
            var taskId = _cpuTaskManager.StartNewTask(request.Complexity);
            return Ok(new { taskId });
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            // Use taskId to identify and stop appropriately. Ensure safe disposal of resources in task manager.
            if (!_cpuTaskManager.StopTask(taskId))
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            // GetActiveTasks must manage memory appropriately; assume CPUTaskManager ensures no leakage.
            var tasks = _cpuTaskManager.GetActiveTasks();
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            // Ensure StopAllTasks disposes relevant resources and avoids memory leaks.
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
        }

        protected override void Dispose(bool disposing)
        {
            // Only dispose managed resources if necessary for ControllerBase subclass.
            if (disposing)
            {
                // Check if _cpuTaskManager has IDisposable implementation and dispose if applicable.
                (_cpuTaskManager as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}