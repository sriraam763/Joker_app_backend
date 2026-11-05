using System.Security.Claims;
using Joker_app_backend.Data;
using Joker_app_backend.DTOs;
using Joker_app_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Joker_app_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JokeController(AppDbContext context)
        {
            _context = context;
        }

        private string? GetCurrentUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JokeResponceDto>>> GetAll([FromQuery] string? catagory)
        {
            var query = _context.Jokes.Include((j => j.User)).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(catagory))
            {
                query = query.Where(j => j.Category.ToLower() == catagory.ToLower());
            }

            var jokes = await query.OrderByDescending(j => j.CreatedAt).Select(j => new JokeResponceDto
            {
                Id = j.Id,
                Setup = j.Setup,
                PunchLine = j.PunchLine,
                Category = j.Category,
                CreatedAt = j.CreatedAt,
                UserID = j.UserID,
                AuthorName = j.User != null ? j.User.FullName : null
            }).ToListAsync();

            return Ok(jokes);
        }

        [HttpGet("random")]
        public async Task<ActionResult<JokeResponceDto>> Getrandom()
        {
            var count = await _context.Jokes.CountAsync();
            if (count == 0)
            {
                return NotFound(new { message = "No jokes found." });
            }
            
            var randomSkip = Random.Shared.Next(0, count);
            var joke = await _context.Jokes
                .Include(j => j.User)
                .AsNoTracking()
                .Skip(randomSkip)
                .Select(j => new JokeResponceDto
                {
                    Id = j.Id,
                    Setup = j.Setup,
                    PunchLine = j.PunchLine,
                    Category = j.Category,
                    CreatedAt = j.CreatedAt,
                    UserID = j.UserID,
                    AuthorName = j.User != null ? j.User.FullName : null
                })
                .FirstOrDefaultAsync();

            return Ok(joke);
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<JokeResponceDto>> GetById(int id)
        {
            var joke = await _context.Jokes
                .Include(j => j.User)
                .AsNoTracking()
                .Where(j => j.Id == id)
                .Select(j => new JokeResponceDto
                {
                    Id = j.Id,
                    Setup = j.Setup,
                    PunchLine = j.PunchLine,
                    Category = j.Category,
                    CreatedAt = j.CreatedAt,
                    UserID = j.UserID,
                    AuthorName = j.User != null ? j.User.FullName : null
                })
                .FirstOrDefaultAsync();
            if (joke == null)
            {
                return NotFound(new { message = $"Joke with id {id} not found." });
            }

            return Ok(joke);
        }
        
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<JokeResponceDto>>> GetMyJokes()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var myJokes = await _context.Jokes
                .Include(j => j.User)
                .AsNoTracking()
                .Where(j => j.UserID == userId)
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JokeResponceDto
                {
                    Id = j.Id,
                    Setup = j.Setup,
                    PunchLine = j.PunchLine,
                    Category = j.Category,
                    CreatedAt = j.CreatedAt,
                    UserID = j.UserID,
                    AuthorName = j.User != null ? j.User.FullName : null
                }).ToListAsync();

            return Ok(myJokes);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<JokeResponceDto>> Create([FromBody] CreateJokeDto dto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var joke = new Joke
            {
                Setup = dto.Setup,
                PunchLine = dto.PunchLine,
                Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category,
                UserID = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Jokes.Add(joke);
            await _context.SaveChangesAsync();

            var author = await _context.Users.Where(u => u.Id == userId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();

            var response = new JokeResponceDto
            {
                Id = joke.Id,
                Setup = joke.Setup,
                PunchLine = joke.PunchLine,
                Category = joke.Category,
                CreatedAt = joke.CreatedAt,
                UserID = joke.UserID,
                AuthorName = author
            };

            return CreatedAtAction(nameof(GetById), new { id = joke.Id }, response);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJokeDto dto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var joke = await _context.Jokes.FindAsync(id);
            if (joke == null)
            {
                return NotFound(new { message = "Joke not Found" });
            }

            joke.Setup = dto.Setup;
            joke.PunchLine = dto.PunchLine;
            joke.Category = string.IsNullOrWhiteSpace(dto.Category) ? "general" : dto.Category;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var joke = await _context.Jokes.FindAsync(id);
            if (joke == null)
            {
                return NotFound(new { message = $"Joke with id {id} not found." });
            }
            
            if (joke.UserID != userId)
            {
                return Forbid();
            }
            _context.Jokes.Remove(joke);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
