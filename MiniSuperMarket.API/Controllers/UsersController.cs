using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();
        public UsersController(SupermarketDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Users.AsNoTracking().OrderBy(x => x.Username)
            .Select(x => new UserDto(x.UserId, x.Username, x.FullName, x.Role, x.IsActive)).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(UserInputDto input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var username = input.Username.Trim();
            if (await _context.Users.AnyAsync(x => x.Username == username)) return Conflict(new { message = "Tên đăng nhập đã tồn tại." });
            var role = NormalizeRole(input.Role);
            if (role == null) return BadRequest(new { message = "Vai trò phải là Admin, Cashier hoặc Warehouse." });
            var user = new User { Username=username, FullName=input.FullName.Trim(), Role=role, IsActive=true };
            user.PasswordHash = _hasher.HashPassword(user, input.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto(user.UserId, user.Username, user.FullName, user.Role, user.IsActive));
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] UserStatusDto input)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (User.Identity?.Name == user.Username && !input.IsActive) return BadRequest(new { message = "Không thể tự khóa tài khoản đang đăng nhập." });
            user.IsActive = input.IsActive;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static string? NormalizeRole(string role) =>
            new[] { "Admin", "Cashier", "Warehouse" }.FirstOrDefault(x => string.Equals(x, role.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public sealed record UserDto(int UserId, string Username, string FullName, string Role, bool IsActive);
    public sealed class UserInputDto
    {
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(50)] public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100)] public string FullName { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required] public string Role { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(6)] public string Password { get; set; } = string.Empty;
    }
    public sealed class UserStatusDto { public bool IsActive { get; set; } }
}
