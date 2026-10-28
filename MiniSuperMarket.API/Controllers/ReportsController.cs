using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Data;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;
        private readonly string _connectionString;

        public ReportsController(SupermarketDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Thiếu ConnectionStrings:DefaultConnection.");
        }

        [HttpGet("quick")]
        public async Task<IActionResult> Quick([FromQuery] DateOnly? date)
        {
            var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var start = selectedDate.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);
            var orders = _context.Orders.AsNoTracking().Where(x => x.CreatedDate >= start && x.CreatedDate < end);
            var summary = await orders.GroupBy(_ => 1).Select(g => new
            {
                TotalOrders = g.Count(),
                TotalRevenue = g.Sum(x => x.FinalAmount)
            }).SingleOrDefaultAsync();
            var bestSeller = await _context.OrderDetails.AsNoTracking()
                .Where(x => x.Order != null && x.Order.CreatedDate >= start && x.Order.CreatedDate < end)
                .GroupBy(x => x.Product!.ProductName)
                .Select(g => new { ProductName = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Quantity).ThenBy(x => x.ProductName).FirstOrDefaultAsync();
            return Ok(new
            {
                Date = selectedDate,
                TotalOrders = summary?.TotalOrders ?? 0,
                TotalRevenue = summary?.TotalRevenue ?? 0m,
                BestSeller = bestSeller?.ProductName ?? "Chưa có dữ liệu",
                BestSellerQuantity = bestSeller?.Quantity ?? 0
            });
        }

        // ADO.NET + Stored Procedure: báo cáo gộp theo từng thu ngân.
        [HttpGet("revenue-by-cashier")]
        public Task<IActionResult> RevenueByCashier() =>
            ReadCashierRevenueAsync("dbo.sp_GetRevenueByCashier", null, null);

        // Bài tập mở rộng: Stored Procedure có tham số ngày bắt đầu/kết thúc.
        [HttpGet("revenue-by-cashier/range")]
        public async Task<IActionResult> RevenueByCashierRange([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            if (startDate > endDate)
                return BadRequest(new { message = "startDate phải nhỏ hơn hoặc bằng endDate." });
            var start = startDate.ToDateTime(TimeOnly.MinValue);
            var endExclusive = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);
            return await ReadCashierRevenueAsync("dbo.sp_GetRevenueByCashierByDateRange", start, endExclusive);
        }

        // Bài tập mở rộng: top 5 sản phẩm bán chạy dùng Stored Procedure.
        [HttpGet("top-selling-products")]
        public async Task<IActionResult> TopSellingProducts()
        {
            var data = new List<TopSellingProductDto>();
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand("dbo.sp_GetTopSellingProducts", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                data.Add(new TopSellingProductDto(
                    reader.GetInt32(reader.GetOrdinal("ProductId")),
                    reader.GetString(reader.GetOrdinal("ProductName")),
                    reader.GetInt32(reader.GetOrdinal("QuantitySold")),
                    reader.GetDecimal(reader.GetOrdinal("Revenue"))));
            }
            return Ok(data);
        }

        private async Task<IActionResult> ReadCashierRevenueAsync(string procedure, DateTime? start, DateTime? endExclusive)
        {
            var data = new List<CashierRevenueDto>();
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(procedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            if (start.HasValue && endExclusive.HasValue)
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = start.Value;
                command.Parameters.Add("@EndDateExclusive", SqlDbType.DateTime2).Value = endExclusive.Value;
            }

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                data.Add(new CashierRevenueDto(
                    reader.IsDBNull(reader.GetOrdinal("CashierUsername")) ? string.Empty : reader.GetString(reader.GetOrdinal("CashierUsername")),
                    reader.IsDBNull(reader.GetOrdinal("TotalOrders")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    reader.IsDBNull(reader.GetOrdinal("TotalRevenue")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    reader.IsDBNull(reader.GetOrdinal("TotalPointsAwarded")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalPointsAwarded"))));
            }

            return Ok(new
            {
                success = true,
                generatedAt = DateTime.Now,
                totalCashiers = data.Count,
                grandTotalRevenue = data.Sum(x => x.TotalRevenue),
                data
            });
        }
    }
}
