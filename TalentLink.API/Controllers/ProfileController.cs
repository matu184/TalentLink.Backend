using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly TalentLinkDbContext _context;

        public ProfileController(TalentLinkDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var id = Guid.Parse(userId);

            var user = await _context.Users
                .Include(u => u.CreatedJobs)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            List<Job>? appliedJobs = null;
            List<VerifiedStudent>? verifiedStudents = null;

            if (user is Student student)
            {
                appliedJobs = await _context.JobApplications
                    .Where(a => a.StudentId == student.Id)
                    .Include(a => a.Job)
                    .Select(a => a.Job)
                    .ToListAsync();
            }
            else if (user is Parent parent)
            {
                verifiedStudents = await _context.Students
                    .Where(s => s.VerifiedByParentId == parent.Id)
                    .Select(s => new VerifiedStudent
                    {
                        StudentId = s.Id,
                        ParentId = parent.Id
                    })
                    .ToListAsync();
            }

            var dto = new ProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedJobs = user is Senior ? user.CreatedJobs.ToList() : null,
                AppliedJobs = appliedJobs,
                VerifiedStudents = verifiedStudents
            };

            return Ok(dto);
        }
    }
}

