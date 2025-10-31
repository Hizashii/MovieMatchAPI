using MovieMatch.Api.Models;

namespace MovieMatch.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
    }
}

