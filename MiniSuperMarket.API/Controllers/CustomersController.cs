using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")] // Khách hàng: Admin và Thu ngân được thao tác
    public class CustomersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public CustomersController(SupermarketDbContext context)
        {
            _context = context;
        }

        private static string NormalizeRank(string? rank) =>
            string.IsNullOrWhiteSpace(rank) ? "Chuẩn" : rank.Trim();

        // GET /api/customers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Customers.AsNoTracking()
                .OrderBy(c => c.CustomerId)
                .ToListAsync();
            return Ok(list);
        }

        // GET /api/customers/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });
            return Ok(customer);
        }

        // GET /api/customers/search?keyword=...  (theo tên hoặc số điện thoại)
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm!" });

            var kw = keyword.Trim();
            var result = await _context.Customers.AsNoTracking()
                .Where(c => c.CustomerName.Contains(kw) || c.PhoneNumber.Contains(kw))
                .OrderBy(c => c.CustomerId)
                .ToListAsync();
            return Ok(result);
        }

        // POST /api/customers
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer newCustomer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            newCustomer.PhoneNumber = newCustomer.PhoneNumber.Trim();
            if (await _context.Customers.AnyAsync(c => c.PhoneNumber == newCustomer.PhoneNumber))
                return Conflict(new { message = "Số điện thoại này đã được đăng ký thành viên!" });

            newCustomer.CustomerId = 0; // Để SQL Server tự sinh Identity
            newCustomer.CustomerName = newCustomer.CustomerName.Trim();
            newCustomer.MembershipRank = NormalizeRank(newCustomer.MembershipRank);

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        // PUT /api/customers/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (update.CustomerId != 0 && update.CustomerId != id)
                return BadRequest(new { message = "Mã khách hàng trong đường dẫn và nội dung không khớp!" });

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng cần sửa!" });

            var phone = update.PhoneNumber.Trim();
            if (await _context.Customers.AnyAsync(c => c.PhoneNumber == phone && c.CustomerId != id))
                return Conflict(new { message = "Số điện thoại này đã thuộc về khách hàng khác!" });

            customer.CustomerName = update.CustomerName.Trim();
            customer.PhoneNumber = phone;
            customer.Address = update.Address;
            customer.RewardPoints = update.RewardPoints;
            customer.MembershipRank = NormalizeRank(update.MembershipRank);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/customers/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng cần xóa!" });

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
