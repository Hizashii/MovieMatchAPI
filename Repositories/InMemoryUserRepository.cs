using System.Collections.Concurrent;
using MovieMatch.Api.Models;

namespace MovieMatch.Api.Repositories
{
    public sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _usersByUsername = new();
        private int _nextId = 1;
        private readonly object _lock = new();

        public Task<User?> GetByUsernameAsync(string username)
        {
            _usersByUsername.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }

        public Task<User> AddAsync(User user)
        {
            lock (_lock)
            {
                user.Id = _nextId++;
                _usersByUsername[user.Username] = user;
                return Task.FromResult(user);
            }
        }
    }
}

