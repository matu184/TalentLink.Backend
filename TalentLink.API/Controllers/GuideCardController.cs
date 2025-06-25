using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class GuideCardController : ControllerBase
    {
        private readonly TalentLinkDbContext _context;

        public GuideCardController(TalentLinkDbContext context)
        {
            _context = context;
        }

        // GET: api/GuideCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuideCard>>> GetGuideCards()
        {
            try
            {
                var guideCards = await _context.GuideCards
                    .OrderBy(g => g.OrderPosition)
                    .ToListAsync();
                return Ok(guideCards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/GuideCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GuideCard>> GetGuideCard(int id)
        {
            try
            {
                var guideCard = await _context.GuideCards.FindAsync(id);

                if (guideCard == null)
                {
                    return NotFound($"GuideCard with ID {id} not found.");
                }

                return Ok(guideCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/GuideCards
        [HttpPost]
        public async Task<ActionResult<GuideCard>> CreateGuideCard([FromBody] GuideCardCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guideCard = new GuideCard
                {
                    Type = dto.Type,
                    Content = dto.Content,
                    OrderPosition = dto.OrderPosition,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.GuideCards.Add(guideCard);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGuideCard), new { id = guideCard.Id }, guideCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/GuideCards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuideCard(int id, [FromBody] GuideCardUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guideCard = await _context.GuideCards.FindAsync(id);
                if (guideCard == null)
                {
                    return NotFound($"GuideCard with ID {id} not found.");
                }

                guideCard.Type = dto.Type;
                guideCard.Content = dto.Content;
                guideCard.OrderPosition = dto.OrderPosition;
                guideCard.UpdatedAt = DateTime.UtcNow;

                _context.Entry(guideCard).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(guideCard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/GuideCards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuideCard(int id)
        {
            try
            {
                var guideCard = await _context.GuideCards.FindAsync(id);
                if (guideCard == null)
                {
                    return NotFound($"GuideCard with ID {id} not found.");
                }

                _context.GuideCards.Remove(guideCard);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // DTOs for API requests
    public class GuideCardCreateDto
    {
        [Required(ErrorMessage = "Type is required")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "OrderPosition is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderPosition must be at least 1")]
        public int OrderPosition { get; set; }
    }

    public class GuideCardUpdateDto
    {
        [Required(ErrorMessage = "Type is required")]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "OrderPosition is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderPosition must be at least 1")]
        public int OrderPosition { get; set; }
    }
}
