using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly TalentLinkDbContext _context;

    public RatingController(TalentLinkDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var fromUserId = Guid.Parse(userId);

        if (dto.Score < 1 || dto.Score > 5)
            return BadRequest("Score muss zwischen 1 und 5 liegen.");

        if (fromUserId == dto.ToUserId)
            return BadRequest("Du kannst dich nicht selbst bewerten.");

        var alreadyRated = await _context.Ratings
            .AnyAsync(r => r.FromUserId == fromUserId && r.ToUserId == dto.ToUserId);

        if (alreadyRated)
            return BadRequest("Du hast diesen Benutzer bereits bewertet.");

        var rating = new Rating
        {
            Id = Guid.NewGuid(),
            FromUserId = fromUserId,
            ToUserId = dto.ToUserId,
            Score = dto.Score,
            Comment = dto.Comment
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return Ok("Bewertung wurde gespeichert.");
    }
}
