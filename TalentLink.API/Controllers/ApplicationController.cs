using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Domain.Entities;
using TalentLink.Domain.Enums;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly TalentLinkDbContext _context;

    public ApplicationController(TalentLinkDbContext context)
    {
        _context = context;
    }

    [HttpPost("{jobId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Apply(Guid jobId, [FromBody] ApplyJobDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == Guid.Parse(userId));
        if (student == null) return Unauthorized("Kein Student gefunden.");

        
        bool isVerified = await _context.VerifiedStudents
            .AnyAsync(v => v.StudentId == student.Id);

        if (student.VerifiedByParentId == null)
            return Forbid("Du wurdest noch nicht von einem Elternteil verifiziert.");

        // Doppelte Bewerbung verhindern
        bool alreadyApplied = await _context.JobApplications
            .AnyAsync(a => a.JobId == jobId && a.StudentId == student.Id);

        if (alreadyApplied)
            return BadRequest("Du hast dich bereits auf diesen Job beworben.");

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            StudentId = student.Id,
            Message = dto.Message,
            AppliedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Pending
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        return Ok("Bewerbung erfolgreich abgeschickt.");
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Senior")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] ApplicationStatus status)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var app = await _context.JobApplications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();
        if (app.Job.CreatedById != Guid.Parse(userId))
            return Forbid("Nur der Ersteller des Jobs darf Bewerbungen verwalten.");

        app.Status = status;

        if (status == ApplicationStatus.Accepted)
        {
            app.Job.IsAssigned = true;

            var andereBewerbungen = await _context.JobApplications
                .Where(a => a.JobId == app.JobId && a.Id != app.Id)
                .ToListAsync();

            foreach (var other in andereBewerbungen)
            {
                other.Status = ApplicationStatus.Rejected;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { status = app.Status.ToString() });
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyApplications()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var applications = await _context.JobApplications
            .Include(a => a.Job)
            .Where(a => a.StudentId == Guid.Parse(userId))
            .Select(a => new MyApplicationDto
            {
                ApplicationId = a.Id,
                JobId = a.JobId,
                JobTitle = a.Job.Title,
                Message = a.Message,
                AppliedAt = a.AppliedAt,
                Status = a.Status
            })
            .ToListAsync();

        return Ok(applications);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> WithdrawApplication(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var app = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == Guid.Parse(userId));

        if (app == null)
            return NotFound("Bewerbung nicht gefunden oder du bist nicht berechtigt.");

        if (app.Status != ApplicationStatus.Pending)
            return BadRequest("Nur Bewerbungen mit Status 'Pending' dürfen zurückgezogen werden.");

        _context.JobApplications.Remove(app);
        await _context.SaveChangesAsync();

        return Ok("Bewerbung wurde zurückgezogen.");
    }
}
