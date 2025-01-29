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
            _cpuTaskManager = cpuTaskManager ?? throw new ArgumentNullException(nameof(cpuTaskManager));
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

            // Clear completed tasks from memory to reduce retention issues
            _cpuTaskManager.ClearCompletedTasks();

            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            _cpuTaskManager.StopAllTasks();

            // Ensure all resources tied to tasks are cleaned up
            _cpuTaskManager.DisposeAll();

            return Ok(new { message = "All tasks stopped" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Properly dispose of the CPU Task Manager to release resources
                _cpuTaskManager?.Dispose(); 
            }
            base.Dispose(disposing);
        }
    }
}