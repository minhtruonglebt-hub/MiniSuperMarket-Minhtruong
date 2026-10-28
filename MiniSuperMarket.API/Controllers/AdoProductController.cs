using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MiniSuperMarket.API.Controllers;

/// <summary>
/// Buổi 5: các ví dụ truy cập SQL Server trực tiếp bằng ADO.NET.
/// EF Core vẫn được dùng cho các nghiệp vụ hiện có ở Buổi 3 và Buổi 4.
/// </summary>
[ApiController]
[Route("api/ado-products")]
[Authorize(Roles = "Admin,Cashier,Warehouse")]
public sealed class AdoProductController : ControllerBase
{
    private readonly string _connectionString;

    public AdoProductController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Thiếu ConnectionStrings:DefaultConnection.");
    }

    // ExecuteScalar: lấy một giá trị đơn, phù hợp cho thẻ tổng số sản phẩm.
    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("SELECT COUNT(*) FROM dbo.Products;", connection);
        await connection.OpenAsync();
        var total = Convert.ToInt32(await command.ExecuteScalarAsync());
        return Ok(new { totalProducts = total });
    }

    // ExecuteReader: đọc tuần tự các dòng khi cần danh sách hiện thời.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string sql = "SELECT ProductId, Barcode, ProductName, Price, StockQuantity FROM dbo.Products ORDER BY ProductId;";
        var products = new List<ProductLookupDto>();
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        while (await reader.ReadAsync()) products.Add(ReadProduct(reader));
        return Ok(products);
    }

    // Connected mode: mở connection, đọc một sản phẩm rồi giải phóng ngay.
    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetProductByBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return BadRequest(new { message = "Vui lòng nhập mã vạch sản phẩm." });

        const string sql = "SELECT ProductId, Barcode, ProductName, Price, StockQuantity FROM dbo.Products WHERE Barcode = @Barcode;";
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Barcode", barcode.Trim());
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await reader.ReadAsync())
            return NotFound(new { message = "Không tìm thấy sản phẩm có mã vạch này." });
        return Ok(ReadProduct(reader));
    }

    // Disconnected mode: SqlDataAdapter.Fill tự mở/đóng connection; DataTable giữ snapshot trong RAM.
    [HttpGet("disconnected-cache")]
    public IActionResult GetDisconnectedSnapshot()
    {
        const string sql = "SELECT ProductId, Barcode, ProductName, Price, StockQuantity FROM dbo.Products ORDER BY ProductId;";
        var table = new DataTable("Products");
        using (var connection = new SqlConnection(_connectionString))
        using (var adapter = new SqlDataAdapter(sql, connection))
            adapter.Fill(table);

        var data = table.Rows.Cast<DataRow>().Select(row => new ProductLookupDto(
            Convert.ToInt32(row["ProductId"]),
            Convert.ToString(row["Barcode"]) ?? string.Empty,
            Convert.ToString(row["ProductName"]) ?? string.Empty,
            Convert.ToDecimal(row["Price"]),
            Convert.ToInt32(row["StockQuantity"]))).ToList();
        return Ok(new { totalLoaded = data.Count, cachedAt = DateTime.Now, data });
    }

    // Thực hành SqlParameter: không nối giá trị người dùng vào câu SQL.
    [HttpGet("filter-stock")]
    public async Task<IActionResult> FilterByStock([FromQuery] int minStock = 50)
    {
        if (minStock < 0) return BadRequest(new { message = "minStock phải lớn hơn hoặc bằng 0." });
        const string sql = "SELECT ProductId, Barcode, ProductName, Price, StockQuantity FROM dbo.Products WHERE StockQuantity <= @MinStock ORDER BY StockQuantity, ProductId;";
        var products = new List<ProductLookupDto>();
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MinStock", SqlDbType.Int).Value = minStock;
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) products.Add(ReadProduct(reader));
        return Ok(new { minStock, total = products.Count, data = products });
    }

    // SqlDataAdapter + DataTable: tổng hợp số sản phẩm trong từng nhóm hàng.
    [HttpGet("category-counts")]
    public IActionResult GetCategoryCounts()
    {
        const string sql = """
            SELECT c.CategoryId, c.CategoryName, COUNT(p.ProductId) AS Total
            FROM dbo.Categories AS c
            LEFT JOIN dbo.Products AS p ON p.CategoryId = c.CategoryId
            GROUP BY c.CategoryId, c.CategoryName
            ORDER BY c.CategoryId;
            """;
        var table = new DataTable("CategoryCounts");
        using (var connection = new SqlConnection(_connectionString))
        using (var adapter = new SqlDataAdapter(sql, connection))
            adapter.Fill(table);

        var data = table.Rows.Cast<DataRow>().Select(row => new CategoryProductCountDto(
            Convert.ToInt32(row["CategoryId"]),
            Convert.ToString(row["CategoryName"]) ?? string.Empty,
            Convert.ToInt32(row["Total"]))).ToList();
        return Ok(data);
    }

    // ExecuteNonQuery: cập nhật tồn kho bằng tham số; thao tác này dành cho Admin/Thủ kho.
    [HttpPut("{id:int}/stock")]
    [Authorize(Roles = "Admin,Warehouse")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        const string sql = "UPDATE dbo.Products SET StockQuantity = @StockQuantity WHERE ProductId = @ProductId;";
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@StockQuantity", SqlDbType.Int).Value = input.StockQuantity;
        command.Parameters.Add("@ProductId", SqlDbType.Int).Value = id;
        await connection.OpenAsync();
        var affected = await command.ExecuteNonQueryAsync();
        return affected == 0
            ? NotFound(new { message = "Không tìm thấy sản phẩm cần cập nhật tồn kho." })
            : Ok(new { productId = id, stockQuantity = input.StockQuantity });
    }

    private static ProductLookupDto ReadProduct(SqlDataReader reader) => new(
        reader.GetInt32(reader.GetOrdinal("ProductId")),
        reader.GetString(reader.GetOrdinal("Barcode")),
        reader.GetString(reader.GetOrdinal("ProductName")),
        reader.GetDecimal(reader.GetOrdinal("Price")),
        reader.GetInt32(reader.GetOrdinal("StockQuantity")));
}

public sealed record ProductLookupDto(int ProductId, string Barcode, string ProductName, decimal Price, int StockQuantity);
public sealed record CategoryProductCountDto(int CategoryId, string CategoryName, int Total);
public sealed class StockUpdateDto
{
    [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
}
