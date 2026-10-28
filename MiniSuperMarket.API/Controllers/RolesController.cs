using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Quản lý vai trò: chỉ Admin
    public class RolesController : ControllerBase
    {
        // Dữ liệu mẫu lưu tạm trên RAM (bài tập mở rộng Buổi 1)
        private static readonly List<Role> _roles = new()
        {
            new Role { Id = 1, RoleName = "Admin", Description = "Quản trị viên toàn hệ thống, toàn quyền thao tác" },
            new Role { Id = 2, RoleName = "Cashier", Description = "Nhân viên thu ngân, chuyên trách bán hàng tại quầy POS" },
            new Role { Id = 3, RoleName = "Warehouse", Description = "Nhân viên quản lý kho, nhập hàng và kiểm kê" }
        };

        // GET /api/roles
        [HttpGet]
        public IActionResult GetAll() => Ok(_roles);

        // GET /api/roles/1
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return NotFound(new { message = "Không tìm thấy vai trò!" });
            return Ok(role);
        }

        // POST /api/roles
        [HttpPost]
        public IActionResult Create([FromBody] Role newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole.RoleName))
                return BadRequest(new { message = "Tên vai trò không được để trống!" });

            newRole.Id = _roles.Count > 0 ? _roles.Max(r => r.Id) + 1 : 1;
            _roles.Add(newRole);
            return CreatedAtAction(nameof(GetById), new { id = newRole.Id }, newRole);
        }

        // PUT /api/roles/1
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Role updateRole)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return NotFound(new { message = "Không tìm thấy vai trò cần sửa!" });
            if (string.IsNullOrWhiteSpace(updateRole.RoleName))
                return BadRequest(new { message = "Tên vai trò không được để trống!" });

            role.RoleName = updateRole.RoleName;
            role.Description = updateRole.Description;
            return NoContent();
        }

        // DELETE /api/roles/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return NotFound(new { message = "Không tìm thấy vai trò cần xóa!" });

            _roles.Remove(role);
            return NoContent();
        }
    }
}
