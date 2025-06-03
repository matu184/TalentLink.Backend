using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;
using TalentLink.Application.DTOs;

namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class TipsController : ControllerBase
    {
        private readonly TalentLinkDbContext _context;

        public TipsController(TalentLinkDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var tips = await _context.Tips
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(tips);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipCreateDto tip)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var newTip = new Tip
            {
                Id = Guid.NewGuid(),
                Title = tip.Title,
                Content = tip.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedById = Guid.Parse(userId)
            };

            _context.Tips.Add(newTip);
            await _context.SaveChangesAsync();

            return Ok(newTip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tip = await _context.Tips.FindAsync(id);
            if (tip == null) return NotFound();

            _context.Tips.Remove(tip);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(Guid id, [FromBody] TipUpdateDto updated)
        {
            var tip = await _context.Tips.FindAsync(id);
            if (tip == null) return NotFound();

            tip.Title = updated.Title;
            tip.Content = updated.Content;
            await _context.SaveChangesAsync();

            return Ok(tip);
        }
    }
}
