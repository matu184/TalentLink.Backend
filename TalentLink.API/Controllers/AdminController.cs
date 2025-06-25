using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly TalentLinkDbContext _context;

    public AdminController(TalentLinkDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userCounts = await _context.Users
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var totalJobs = await _context.Jobs.CountAsync();
        var totalApplications = await _context.JobApplications.CountAsync();
        var totalComments = await _context.JobComments.CountAsync();
        var totalRatings = await _context.Ratings.CountAsync();
        var totalTips = await _context.Tips.CountAsync();

        return Ok(new
        {
            Users = userCounts,
            TotalJobs = totalJobs,
            TotalApplications = totalApplications,
            TotalComments = totalComments,
            TotalRatings = totalRatings,
            TotalTips = totalTips
        });
    }
}
