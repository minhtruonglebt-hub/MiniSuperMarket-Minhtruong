using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;
        public ProductsController(SupermarketDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Products.AsNoTracking()
            .Include(x => x.Category).OrderBy(x => x.ProductName)
            .Select(x => new ProductDto(x.ProductId, x.Barcode, x.ProductName, x.Price, x.StockQuantity, x.CategoryId, x.Category!.CategoryName))
            .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.AsNoTracking().Include(x => x.Category)
                .Where(x => x.ProductId == id)
                .Select(x => new ProductDto(x.ProductId, x.Barcode, x.ProductName, x.Price, x.StockQuantity, x.CategoryId, x.Category!.CategoryName))
                .SingleOrDefaultAsync();
            return product == null ? NotFound() : Ok(product);
        }

        [HttpGet("barcode/{barcode}")]
        [Authorize(Roles = "Admin,Cashier,Warehouse")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var p = await _context.Products.AsNoTracking().Include(x => x.Category)
                .Where(x => x.Barcode == barcode.Trim())
                .Select(x => new ProductDto(x.ProductId, x.Barcode, x.ProductName, x.Price, x.StockQuantity, x.CategoryId, x.Category!.CategoryName))
                .SingleOrDefaultAsync();
            return p == null ? NotFound(new { message = "Không tìm thấy sản phẩm theo mã vạch." }) : Ok(p);
        }

        [HttpGet("with-category")]
        public async Task<IActionResult> WithCategory() => Ok(await _context.Products.AsNoTracking()
            .Include(x => x.Category).OrderBy(x => x.ProductName)
            .Select(x => new ProductDto(x.ProductId, x.Barcode, x.ProductName, x.Price, x.StockQuantity, x.CategoryId, x.Category!.CategoryName))
            .ToListAsync());

        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create(ProductInputDto input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (!await _context.Categories.AnyAsync(x => x.CategoryId == input.CategoryId)) return BadRequest(new { message = "Nhóm hàng không tồn tại." });
            if (await _context.Products.AnyAsync(x => x.Barcode == input.Barcode.Trim())) return Conflict(new { message = "Mã vạch đã được sử dụng." });
            var product = new Product { Barcode=input.Barcode.Trim(), ProductName=input.ProductName.Trim(), Price=input.Price, StockQuantity=input.StockQuantity, CategoryId=input.CategoryId };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(int id, ProductInputDto input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            if (!await _context.Categories.AnyAsync(x => x.CategoryId == input.CategoryId)) return BadRequest(new { message = "Nhóm hàng không tồn tại." });
            if (await _context.Products.AnyAsync(x => x.Barcode == input.Barcode.Trim() && x.ProductId != id)) return Conflict(new { message = "Mã vạch đã được sử dụng." });
            product.Barcode=input.Barcode.Trim(); product.ProductName=input.ProductName.Trim(); product.Price=input.Price;
            product.StockQuantity=input.StockQuantity; product.CategoryId=input.CategoryId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            if (await _context.OrderDetails.AnyAsync(x => x.ProductId == id)) return Conflict(new { message = "Sản phẩm đã xuất hiện trên hóa đơn, không thể xóa." });
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public sealed record ProductDto(int ProductId, string Barcode, string ProductName, decimal Price, int StockQuantity, int CategoryId, string CategoryName);
    public sealed class ProductInputDto
    {
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(50)] public string Barcode { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(150)] public string ProductName { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Range(0, double.MaxValue)] public decimal Price { get; set; }
        [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)] public int StockQuantity { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)] public int CategoryId { get; set; }
    }
}
