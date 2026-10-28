using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")]
    public class OrdersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;
        public OrdersController(SupermarketDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var requested = input.Items.GroupBy(x => x.ProductId).Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) }).ToList();
            if (requested.Count == 0 || requested.Any(x => x.Quantity <= 0)) return BadRequest(new { message = "Giỏ hàng không có sản phẩm hợp lệ." });

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var products = await _context.Products.Where(x => requested.Select(i => i.ProductId).Contains(x.ProductId)).ToDictionaryAsync(x => x.ProductId);
            if (products.Count != requested.Count) return BadRequest(new { message = "Có sản phẩm không còn tồn tại." });
            var shortage = requested.FirstOrDefault(x => products[x.ProductId].StockQuantity < x.Quantity);
            if (shortage != null) return Conflict(new { message = $"Tồn kho không đủ cho sản phẩm {products[shortage.ProductId].ProductName}." });

            Customer? customer = null;
            if (input.CustomerId.HasValue)
            {
                customer = await _context.Customers.FindAsync(input.CustomerId.Value);
                if (customer == null) return BadRequest(new { message = "Không tìm thấy khách hàng." });
            }

            var order = new Order
            {
                OrderCode = $"HD{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}",
                CreatedDate = DateTime.Now,
                CustomerId = customer?.CustomerId,
                CashierUsername = User.Identity?.Name ?? "unknown",
                TotalItems = requested.Sum(x => x.Quantity)
            };
            foreach (var line in requested)
            {
                var product = products[line.ProductId];
                product.StockQuantity -= line.Quantity;
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = product.ProductId, Quantity = line.Quantity,
                    UnitPrice = product.Price, LineTotal = product.Price * line.Quantity
                });
                order.FinalAmount += product.Price * line.Quantity;
            }
            order.RewardPoints = (int)(order.FinalAmount / 10000m);
            if (customer != null) customer.RewardPoints += order.RewardPoints;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new { order.OrderId, order.OrderCode, order.FinalAmount, order.TotalItems, order.RewardPoints, CustomerRewardPoints = customer?.RewardPoints });
        }
    }

    public sealed class CheckoutDto
    {
        public int? CustomerId { get; set; }
        [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(1)]
        public List<CheckoutLineDto> Items { get; set; } = new();
    }
    public sealed class CheckoutLineDto
    {
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)] public int ProductId { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1, 9999)] public int Quantity { get; set; }
    }
}
