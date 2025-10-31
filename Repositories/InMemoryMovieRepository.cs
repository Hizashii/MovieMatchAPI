using System.Collections.Concurrent;
using MovieMatch.Api.Models;

namespace MovieMatch.Api.Repositories
{
    public sealed class InMemoryMovieRepository : IMovieRepository
    {
        private readonly ConcurrentDictionary<int, Movie> _movies = new();
        private int _nextId = 1;
        private readonly object _lock = new();
        private readonly Random _rng = new();

        public InMemoryMovieRepository()
        {
            // Seed a few
            AddAsync(new Movie { Title = "Inception", Genre = "Sci-Fi", Year = 2010, Rating = 8.8 }).Wait();
            AddAsync(new Movie { Title = "The Dark Knight", Genre = "Action", Year = 2008, Rating = 9.0 }).Wait();
            AddAsync(new Movie { Title = "Interstellar", Genre = "Sci-Fi", Year = 2014, Rating = 8.6 }).Wait();
        }

        public Task<IReadOnlyList<Movie>> GetAllAsync(string? genre = null)
        {
            IEnumerable<Movie> query = _movies.Values;
            if (!string.IsNullOrWhiteSpace(genre))
            {
                var g = genre.ToLower();
                query = query.Where(m => m.Genre != null && m.Genre.ToLower().Contains(g));
            }
            return Task.FromResult((IReadOnlyList<Movie>)query.OrderBy(m => m.Title).ToList());
        }

        public Task<Movie?> GetByIdAsync(int id)
        {
            _movies.TryGetValue(id, out var movie);
            return Task.FromResult(movie);
        }

        public Task<Movie> AddAsync(Movie movie)
        {
            lock (_lock)
            {
                movie.Id = _nextId++;
                _movies[movie.Id] = movie;
                return Task.FromResult(movie);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            var removed = _movies.TryRemove(id, out _);
            return Task.FromResult(removed);
        }

        public Task<IReadOnlyList<Movie>> GetRandomAsync(int count)
        {
            var list = _movies.Values.ToList();
            var take = Math.Min(count, list.Count);
            var sample = list.OrderBy(_ => _rng.Next()).Take(take).ToList();
            return Task.FromResult((IReadOnlyList<Movie>)sample);
        }

        public Task<IReadOnlyList<Movie>> GetRelatedByGenreAsync(string genre, int take)
        {
            var g = genre.ToLower();
            var list = _movies.Values
                .Where(m => m.Genre != null && m.Genre.ToLower().Contains(g))
                .OrderByDescending(m => m.Rating)
                .Take(take)
                .ToList();
            return Task.FromResult((IReadOnlyList<Movie>)list);
        }
    }
}

