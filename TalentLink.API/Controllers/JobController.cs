using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;

namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpPost]
        [Authorize(Roles = "Senior")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Kein Benutzer erkannt.");

            var job = new Job
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                PricePerHour = dto.PricePerHour,
                IsBoosted = dto.IsBoosted,
                CreatedAt = DateTime.UtcNow,
                CreatedById = Guid.Parse(userId)
            };

            var created = await _jobService.CreateJobAsync(job);
            return Ok(created);
        }
    }
}
