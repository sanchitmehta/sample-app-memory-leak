using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Services;
using System.Linq;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase
    {
        private readonly CPUTaskManager _cpuTaskManager;

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager;
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            string taskId;
            using (var taskManager = _cpuTaskManager)
            {
                taskId = taskManager.StartNewTask(request.Complexity);
            }

            return Ok(new { taskId });
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            bool taskStopped;
            using (var taskManager = _cpuTaskManager)
            {
                taskStopped = taskManager.StopTask(taskId);
            }

            if (!taskStopped)
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            var tasks = _cpuTaskManager.GetActiveTasks();
            if (tasks != null && tasks.Any())
            {
                // Ensure tasks list is cleared after retrieving to free memory
                tasks.Clear();
            }
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            using (var taskManager = _cpuTaskManager)
            {
                taskManager.StopAllTasks();
            }
            return Ok(new { message = "All tasks stopped" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cpuTaskManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}