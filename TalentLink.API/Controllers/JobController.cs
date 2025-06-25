using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.API.Utils;
using TalentLink.API.Utils;
using TalentLink.Application.DTOs;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Migrations;
using TalentLink.Infrastructure.Persistence;
using TalentLink.Infrastructure.Services;

namespace TalentLink.API.Controllers
{
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

        private int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
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

            // Debug: Claims ausgeben
            Console.WriteLine($"[Job-GetAll] IsAuthenticated: {User.Identity?.IsAuthenticated}, Claims: {string.Join(", ", User.Claims.Select(c => c.Type + ":" + c.Value))}");

            // Altersfilter für eingeloggte Students
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int? userAge = null;
            if (!string.IsNullOrEmpty(userId) && userRole == "Student")
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == Guid.Parse(userId));
                if (student != null)
                {
                    userAge = CalculateAge(student.DateOfBirth);
                    Console.WriteLine($"[Job-GetAll] UserId: {userId}, Alter: {userAge}");
                }
                else
                {
                    Console.WriteLine($"[Job-GetAll] UserId: {userId}, Student nicht gefunden!");
                }
            }
            else
            {
                Console.WriteLine($"[Job-GetAll] Kein eingeloggter Student. userId: {userId}, userRole: {userRole}");
            }

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Where(j => !j.MinimumAge.HasValue || (userAge.HasValue && userAge.Value >= j.MinimumAge.Value))
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


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.CreatedBy)
                .Include(j => j.Comments).ThenInclude(c => c.Author)
                .Include(j => j.Applications).ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            // Token optional – nur prüfen, wenn vorhanden
            var userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var isOwner = userId != null && job.CreatedById.ToString() == userId;

            var comments = job.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new JobCommentDto
                {
                    AuthorName = c.Author.Name,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            var applications = isOwner
                ? job.Applications.Select(a => new ApplicationInfoDto
                {
                    StudentName = a.Student.Name,
                    Status = a.Status.ToString()
                }).ToList()
                : null;

            return Ok(new JobDetailsDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                PricePerHour = job.PricePerHour,
                CreatedAt = job.CreatedAt,
                IsBoosted = job.IsBoosted,
                Category = job.Category.Name,
                CategoryImage = job.Category.ImageUrl ?? "",
                CreatedBy = job.CreatedBy.Name,
                Comments = comments,
                Applications = applications
            });
        }

        [HttpPost]
        [Authorize(Roles = "Senior")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Kein Benutzer erkannt.");

            // Kategorie aus der Datenbank laden, nicht das gesendete Objekt verwenden
            var category = await _context.JobCategories.FindAsync(dto.Category.Id);
            if (category == null)
                return BadRequest("Kategorie nicht gefunden.");

            // Hole den eingeloggten User aus der Datenbank
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
            if (user == null)
                return Unauthorized("Benutzer nicht gefunden.");

            var job = new Job
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = category.Id, // Use CategoryId instead of Category object
                PricePerHour = dto.PricePerHour,
                IsBoosted = dto.IsBoosted,
                CreatedAt = DateTime.UtcNow,
                CreatedById = Guid.Parse(userId),
                ZipCode = dto.ZipCode,
                City = user.City, // City aus eingeloggtem User setzen
                IsPaid = true,
                MinimumAge = dto.MinimumAge// Set IsPaid to true when creating a job
            };

            // Hole Koordinaten für den Job und logge sie
            try
            {
                var (lat, lng) = await _geocodingService.GetCoordinatesAsync(job.ZipCode, job.City);
                job.Latitude = lat;
                job.Longitude = lng;
                Console.WriteLine($"[Job-Create] City: {job.City}, Zip: {job.ZipCode}, Lat: {lat}, Lng: {lng}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Geocoding fehlgeschlagen: {ex.Message}");
            }

            var created = await _jobService.CreateJobAsync(job);

            // Return a DTO instead of the full entity to avoid circular references
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
        [HttpGet("created-by-me")]
        [Authorize(Roles = "Senior")]
        public async Task<IActionResult> GetJobsCreatedByMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Altersfilter für eingeloggte Students (optional, falls du es auch hier willst)
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int? userAge = null;
            if (!string.IsNullOrEmpty(userId) && userRole == "Student")
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == Guid.Parse(userId));
                if (student != null)
                    userAge = CalculateAge(student.DateOfBirth);
            }

            var jobs = await _context.Jobs
                .Include(j => j.Category)
                .Where(j => j.CreatedById == Guid.Parse(userId))
                .OrderByDescending(j => j.CreatedAt)
                .Where(j => !userAge.HasValue || !j.MinimumAge.HasValue || userAge.Value >= j.MinimumAge.Value)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.PricePerHour,
                    j.IsBoosted,
                    j.CreatedAt,
                    Category = j.Category.Name,
                    CategoryImage = j.Category.ImageUrl
                })
                .ToListAsync();

            return Ok(jobs);
        }
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchNearbyJobs([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 10)
        {
            var jobs = await _context.Jobs
                .Include(j => j.Category)
                .Where(j => j.IsPaid && !j.IsAssigned)
                .ToListAsync();

            // Altersfilter für eingeloggte Students
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int? userAge = null;
            if (!string.IsNullOrEmpty(userId) && userRole == "Student")
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == Guid.Parse(userId));
                if (student != null)
                    userAge = CalculateAge(student.DateOfBirth);
            }

            var nearbyJobs = jobs
                .Where(j => GeoUtils.DistanceInKm(latitude, longitude, j.Latitude, j.Longitude) <= radiusKm)
                .Where(j => !userAge.HasValue || !j.MinimumAge.HasValue || userAge.Value >= j.MinimumAge.Value)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.PricePerHour,
                    j.IsBoosted,
                    j.IsPaid,
                    j.IsAssigned,
                    j.CreatedAt,
                    Category = j.Category.Name,
                    CategoryImage = j.Category.ImageUrl,
                    j.Latitude,
                    j.Longitude
                })
                .ToList();

            return Ok(nearbyJobs);
        }

        [HttpGet("geocode")]
        [AllowAnonymous]
        public async Task<IActionResult> Geocode([FromQuery] string? query, [FromQuery] string? zipcode, [FromQuery] string? city)
        {
            // Wenn query gesetzt ist, nutze es als city, zipcode bleibt leer
            if (!string.IsNullOrWhiteSpace(query))
            {
                var (lat, lng) = await _geocodingService.GetCoordinatesAsync("", query);
                return Ok(new { Lat = lat, Lng = lng });
            }
            // Sonst wie gehabt
            var (lat2, lng2) = await _geocodingService.GetCoordinatesAsync(zipcode, city);
            return Ok(new { Lat = lat2, Lng = lng2 });
        }

    }
}
