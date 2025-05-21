using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/jobs/{jobId}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly TalentLinkDbContext _context;

    public CommentsController(TalentLinkDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(Guid jobId, [FromBody] CreateJobCommentDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null) return NotFound("Job nicht gefunden");

        var comment = new JobComment
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            AuthorId = Guid.Parse(userId),
            Text = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobComments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(Guid jobId)
    {
        var comments = await _context.JobComments
            .Where(c => c.JobId == jobId)
            .Include(c => c.Author)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new JobCommentDto
            {
                AuthorName = c.Author.Name,
                Text = c.Text,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(comments);
    }
}
