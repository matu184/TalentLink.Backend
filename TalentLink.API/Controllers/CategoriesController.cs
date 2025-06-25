using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentLink.Application.DTOs;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly TalentLinkDbContext _context;

    public CategoriesController(TalentLinkDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.JobCategories
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.ImageUrl
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CategoryDto dto)
    {
        var category = new JobCategory
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ImageUrl = dto.ImageUrl
        };

        _context.JobCategories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto dto)
    {
        var category = await _context.JobCategories.FindAsync(id);
        if (category == null) return NotFound();

        category.Name = dto.Name;
        category.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _context.JobCategories.FindAsync(id);
        if (category == null) return NotFound();

        // Optional: Prüfen, ob noch Jobs mit der Kategorie existieren
        var hasJobs = await _context.Jobs.AnyAsync(j => j.CategoryId == id);
        if (hasJobs)
            return BadRequest("Kategorie kann nicht gelöscht werden – es existieren noch Jobs.");

        _context.JobCategories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok("Kategorie gelöscht.");
    }
}
