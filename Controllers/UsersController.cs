using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using MovieMatch.Api.Models;
using MovieMatch.Api.Services;
using MovieMatch.Api.Repositories;

namespace MovieMatch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly JwtTokenService _jwt;

        public UsersController(IUserRepository users, JwtTokenService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public sealed class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public sealed class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var existsUser = await _users.GetByUsernameAsync(request.Username);
            if (existsUser != null) return Conflict("Username already taken.");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            await _users.AddAsync(user);
            return Created($"/api/users/{user.Id}", new { user.Id, user.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _users.GetByUsernameAsync(request.Username);
            if (user == null) return Unauthorized("Invalid credentials.");
            var ok = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!ok) return Unauthorized("Invalid credentials.");

            var token = _jwt.CreateToken(user.Id, user.Username);
            return Ok(new { token, user = new { user.Id, user.Username } });
        }
    }
}

