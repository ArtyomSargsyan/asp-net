using Microsoft.AspNetCore.Mvc;
using ToDoApi.DTO;
using ToDoApi.Models;
using ToDoApi.Services.Auth;
using ToDoApi.Repositories.Users;
using Microsoft.AspNetCore.Authorization;

namespace ToDoApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly JwtService _jwt;

        public AuthController(IUserRepository repo, JwtService jwt)
        {
            _repo = repo;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (await _repo.ExistsByUsernameAsync(dto.UserName))
                return BadRequest("Username already exists.");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _repo.AddAsync(user);

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _repo.GetByUsernameAsync(dto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _jwt.GenerateToken(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            return Ok(new { UserName = userName });
        }
    }
}
