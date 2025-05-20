using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly TalentLinkDbContext _context;

        public JobController(IJobService jobService, TalentLinkDbContext context)
        {
            _jobService = jobService;
            _context = context;
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

        [HttpPost("{id}/apply")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Apply(Guid id, [FromBody] ApplyJobDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var student = await _context.Users
                .OfType<Student>()
                .Include(s => s.Applications)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (student == null) return Forbid();

            var job = await _context.Jobs
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            // bereits beworben?
            if (job.Applications.Any(a => a.StudentId == student.Id))
                return BadRequest("Du hast dich bereits auf diesen Job beworben.");

            var application = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                StudentId = student.Id,
                AppliedAt = DateTime.UtcNow,
                Message = dto.Message
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            return Ok("Bewerbung gespeichert.");
        }
        [HttpGet("{id}/applications")]
        [Authorize(Roles = "Senior")]
        public async Task<IActionResult> GetApplications(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var job = await _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            if (job.CreatedById != Guid.Parse(userId))
                return Forbid("Du darfst nur Bewerbungen auf deine eigenen Jobs sehen.");

            var result = job.Applications.Select(a => new JobApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                StudentId = a.StudentId,
                StudentName = a.Student.Name,
                Message = a.Message,
                AppliedAt = a.AppliedAt
            }).ToList();

            return Ok(result);
        }

    }
}
