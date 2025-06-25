using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Application.DTOs;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;
using TalentLink.Infrastructure.Services;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly TalentLinkDbContext _context;
    private readonly GeocodingService _geocodingService;

    public JobController(IJobService jobService, TalentLinkDbContext context, GeocodingService geocodingService)
    {
        _jobService = jobService;
        _context = context;
        _geocodingService = geocodingService;
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        return age;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId)
    {
        var query = _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.CreatedBy)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(j => j.CategoryId == categoryId.Value);
        }

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new
            {
                j.Id,
                j.Title,
                j.PricePerHour,
                j.IsBoosted,
                j.IsPaid,
                j.IsAssigned,
                j.CreatedAt,
                Category = j.Category.Name,
                CategoryImage = j.Category.ImageUrl,
                CreatedBy = j.CreatedBy.Name
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpPost]
    [Authorize(Roles = "Senior")]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("Kein Benutzer erkannt.");

        var category = await _context.JobCategories.FindAsync(dto.Category.Id);
        if (category == null)
            return BadRequest("Kategorie nicht gefunden.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user == null)
            return Unauthorized("Benutzer nicht gefunden.");

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = category.Id,
            PricePerHour = dto.PricePerHour,
            IsBoosted = dto.IsBoosted,
            CreatedAt = DateTime.UtcNow,
            CreatedById = Guid.Parse(userId),
            ZipCode = dto.ZipCode,
            City = user.City,
            IsPaid = true,
            MinimumAge = dto.MinimumAge
        };

        try
        {
            var (lat, lng) = await _geocodingService.GetCoordinatesAsync(job.ZipCode, job.City);
            job.Latitude = lat;
            job.Longitude = lng;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Geocoding fehlgeschlagen: {ex.Message}");
        }

        var created = await _jobService.CreateJobAsync(job);

        var result = new
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            PricePerHour = created.PricePerHour,
            IsBoosted = created.IsBoosted,
            CreatedAt = created.CreatedAt,
            CategoryId = category.Id,
            CategoryName = category.Name,
            CategoryImage = category.ImageUrl,
            CreateOrt = created.City,
            Latitude = created.Latitude,
            Longitude = created.Longitude
        };

        return Ok(result);
    }
}
