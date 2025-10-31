using MovieMatch.Api.Models;

namespace MovieMatch.Api.Repositories
{
    public interface IMovieRepository
    {
        Task<IReadOnlyList<Movie>> GetAllAsync(string? genre = null);
        Task<Movie?> GetByIdAsync(int id);
        Task<Movie> AddAsync(Movie movie);
        Task<bool> DeleteAsync(int id);
        Task<IReadOnlyList<Movie>> GetRandomAsync(int count);
        Task<IReadOnlyList<Movie>> GetRelatedByGenreAsync(string genre, int take);
    }
}

