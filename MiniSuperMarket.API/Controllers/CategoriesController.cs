using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc có Token hợp lệ mới gọi được
    public class CategoriesController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        // Tiêm DbContext thông qua Constructor Injection
        public CategoriesController(SupermarketDbContext context)
        {
            _context = context;
        }

        // 1. READ: GET /api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Categories.AsNoTracking()
                .OrderBy(c => c.CategoryId)
                .ToListAsync();
            return Ok(list);
        }

        // 2. READ: GET /api/categories/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Không tìm thấy nhóm hàng trong CSDL!" });
            return Ok(category);
        }

        // 3. SEARCH: GET /api/categories/search?keyword=...
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm!" });

            var kw = keyword.Trim();
            // EF Core dịch Contains thành SQL: WHERE CategoryName LIKE N'%kw%'
            var result = await _context.Categories.AsNoTracking()
                .Where(c => c.CategoryName.Contains(kw))
                .OrderBy(c => c.CategoryId)
                .ToListAsync();
            return Ok(result);
        }

        // 4. CREATE: POST /api/categories
        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create([FromBody] Category newCat)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            newCat.CategoryId = 0; // Để SQL Server tự sinh Identity
            newCat.CategoryName = newCat.CategoryName.Trim();

            _context.Categories.Add(newCat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newCat.CategoryId }, newCat);
        }

        // 5. UPDATE: PUT /api/categories/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updateCat)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (updateCat.CategoryId != 0 && updateCat.CategoryId != id)
                return BadRequest(new { message = "Mã nhóm hàng trong đường dẫn và nội dung không khớp!" });

            var cat = await _context.Categories.FindAsync(id);
            if (cat == null)
                return NotFound(new { message = "Không tìm thấy nhóm hàng cần sửa!" });

            cat.CategoryName = updateCat.CategoryName.Trim();
            cat.Description = updateCat.Description;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE: DELETE /api/categories/1  -> CHỈ ADMIN
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null)
                return NotFound(new { message = "Không tìm thấy nhóm hàng cần xóa!" });

            // Không cho xóa nhóm hàng đang có sản phẩm (bảo toàn dữ liệu)
            if (await _context.Products.AnyAsync(p => p.CategoryId == id))
                return Conflict(new { message = "Nhóm hàng đang có sản phẩm, không thể xóa!" });

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ===== Endpoint kiểm tra phân quyền (Buổi 2) =====

        // Chỉ Admin
        [HttpGet("admin-dashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminDashboard()
        {
            return Ok(new { message = "Chào mừng Admin! Bạn có toàn quyền quản trị hệ thống siêu thị mini." });
        }

        // Admin + Cashier
        [HttpGet("staff-pos")]
        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult GetStaffPos()
        {
            return Ok(new { message = "Màn hình POS Thu ngân sẵn sàng phục vụ bán hàng." });
        }
    }
}
