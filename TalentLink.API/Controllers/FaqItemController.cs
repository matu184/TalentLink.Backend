using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class FaqItemController : ControllerBase
    {
        private readonly TalentLinkDbContext _context;

        public FaqItemController(TalentLinkDbContext context)
        {
            _context = context;
        }

        // GET: api/FaqItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FaqItem>>> GetFaqItems()
        {
            try
            {
                var faqItems = await _context.FaqItems
                    .OrderBy(f => f.OrderPosition)
                    .ToListAsync();
                return Ok(faqItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/FaqItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FaqItem>> GetFaqItem(int id)
        {
            try
            {
                var faqItem = await _context.FaqItems.FindAsync(id);

                if (faqItem == null)
                {
                    return NotFound($"FaqItem with ID {id} not found.");
                }

                return Ok(faqItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/FaqItems
        [HttpPost]
        public async Task<ActionResult<FaqItem>> CreateFaqItem([FromBody] FaqItemCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var faqItem = new FaqItem
                {
                    Question = dto.Question,
                    Answer = dto.Answer,
                    OrderPosition = dto.OrderPosition,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.FaqItems.Add(faqItem);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFaqItem), new { id = faqItem.Id }, faqItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/FaqItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaqItem(int id, [FromBody] FaqItemUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var faqItem = await _context.FaqItems.FindAsync(id);
                if (faqItem == null)
                {
                    return NotFound($"FaqItem with ID {id} not found.");
                }

                faqItem.Question = dto.Question;
                faqItem.Answer = dto.Answer;
                faqItem.OrderPosition = dto.OrderPosition;
                faqItem.UpdatedAt = DateTime.UtcNow;

                _context.Entry(faqItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(faqItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/FaqItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaqItem(int id)
        {
            try
            {
                var faqItem = await _context.FaqItems.FindAsync(id);
                if (faqItem == null)
                {
                    return NotFound($"FaqItem with ID {id} not found.");
                }

                _context.FaqItems.Remove(faqItem);
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
    public class FaqItemCreateDto
    {
        [Required(ErrorMessage = "Question is required")]
        [StringLength(500, ErrorMessage = "Question cannot exceed 500 characters")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Answer is required")]
        [StringLength(2000, ErrorMessage = "Answer cannot exceed 2000 characters")]
        public string Answer { get; set; } = string.Empty;

        [Required(ErrorMessage = "OrderPosition is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderPosition must be at least 1")]
        public int OrderPosition { get; set; }
    }

    public class FaqItemUpdateDto
    {
        [Required(ErrorMessage = "Question is required")]
        [StringLength(500, ErrorMessage = "Question cannot exceed 500 characters")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Answer is required")]
        [StringLength(2000, ErrorMessage = "Answer cannot exceed 2000 characters")]
        public string Answer { get; set; } = string.Empty;

        [Required(ErrorMessage = "OrderPosition is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderPosition must be at least 1")]
        public int OrderPosition { get; set; }
    }
}

