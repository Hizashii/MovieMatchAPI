using Microsoft.AspNetCore.Mvc;
using MovieMatch.Api.Models;
using MovieMatch.Api.Repositories;

namespace MovieMatch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class RecommendationsController : ControllerBase
    {
        private readonly IMovieRepository _repo;

        public RecommendationsController(IMovieRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> Get([FromQuery] string type = "random", [FromQuery] string? genre = null)
        {
            if (type.Equals("related", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(genre))
            {
                var list = await _repo.GetRelatedByGenreAsync(genre, 10);
                return Ok(list);
            }
            var sample = await _repo.GetRandomAsync(10);
            return Ok(sample);
        }
    }
}

