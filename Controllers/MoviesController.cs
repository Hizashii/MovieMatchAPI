using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMatch.Api.Models;
using MovieMatch.Api.Repositories;
using MovieMatch.Api.Filters;

namespace MovieMatch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _repo;

        public MoviesController(IMovieRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies([FromQuery] string? genre)
        {
            var movies = await _repo.GetAllAsync(genre);
            return Ok(movies);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _repo.GetByIdAsync(id);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [ServiceFilter(typeof(RequireAuthUnlessInMemory))]
        [HttpPost]
        public async Task<ActionResult<Movie>> CreateMovie([FromBody] Movie input)
        {
            var movie = new Movie
            {
                Title = input.Title,
                Overview = input.Overview,
                Genre = input.Genre,
                Year = input.Year,
                Rating = input.Rating,
                PosterUrl = input.PosterUrl
            };
            var created = await _repo.AddAsync(movie);
            return CreatedAtAction(nameof(GetMovie), new { id = created.Id }, created);
        }

        [ServiceFilter(typeof(RequireAuthUnlessInMemory))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var removed = await _repo.DeleteAsync(id);
            if (!removed) return NotFound();
            return NoContent();
        }
    }
}

