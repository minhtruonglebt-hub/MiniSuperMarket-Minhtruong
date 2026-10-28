using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Controllers;

/// <summary>Buổi 5: ghi hóa đơn, chi tiết, trừ kho và cộng điểm trong cùng một SqlTransaction.</summary>
[ApiController]
[Route("api/checkout")]
[Authorize(Roles = "Admin,Cashier")]
public sealed class CheckoutController : ControllerBase
{
    private readonly string _connectionString;

    public CheckoutController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Thiếu ConnectionStrings:DefaultConnection.");
    }

    [HttpPost]
    public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequestDto request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (request.Items.Count > 100)
            return BadRequest(new { message = "Một hóa đơn không thể có quá 100 dòng hàng." });

        var items = request.Items
            .GroupBy(x => x.ProductId)
            .Select(g => new CheckoutItemRequestDto { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .OrderBy(x => x.ProductId)
            .ToList();
        if (items.Count == 0 || items.Any(x => x.Quantity is < 1 or > 9999))
            return BadRequest(new { message = "Giỏ hàng không có số lượng sản phẩm hợp lệ." });

        var actor = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(actor)) return Unauthorized();
        var actorIsAdmin = User.IsInRole("Admin");
        var cashierUsername = actorIsAdmin && !string.IsNullOrWhiteSpace(request.CashierUsername)
            ? request.CashierUsername.Trim()
            : actor;
        if (!actorIsAdmin && !string.IsNullOrWhiteSpace(request.CashierUsername) &&
            !string.Equals(request.CashierUsername.Trim(), actor, StringComparison.OrdinalIgnoreCase))
            return Forbid();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            var cashierRole = await GetActiveUserRoleAsync(connection, transaction, cashierUsername);
            if (cashierRole is null || (cashierRole != "Cashier" && !(actorIsAdmin && cashierRole == "Admin")))
                throw new CheckoutRejectedException("Tài khoản lập hóa đơn không tồn tại, đã khóa hoặc không có vai trò hợp lệ.");

            int? customerPointsBefore = null;
            if (request.CustomerId.HasValue)
            {
                customerPointsBefore = await GetCustomerPointsAsync(connection, transaction, request.CustomerId.Value);
                if (!customerPointsBefore.HasValue)
                    throw new CheckoutRejectedException("Không tìm thấy khách hàng đã chọn.");
            }

            var products = new List<CheckoutProduct>();
            foreach (var item in items)
            {
                var product = await GetProductAsync(connection, transaction, item.ProductId);
                if (product is null)
                    throw new CheckoutRejectedException($"Không tìm thấy sản phẩm ID {item.ProductId}.");
                if (product.StockQuantity < item.Quantity)
                    throw new CheckoutRejectedException(
                        $"Sản phẩm '{product.ProductName}' (ID: {product.ProductId}) không đủ tồn kho! (Tồn kho thực tế: {product.StockQuantity}, Số lượng yêu cầu mua: {item.Quantity})");
                product.Quantity = item.Quantity;
                products.Add(product);
            }

            // Giá bán luôn lấy từ database; giá gửi từ client không được dùng để tính tiền.
            var finalAmount = products.Sum(x => x.Price * x.Quantity);
            var totalItems = products.Sum(x => x.Quantity);
            var rewardPoints = (int)(finalAmount / 10000m);
            var orderCode = $"TA{DateTime.Now:yyyyMMddHHmmssfff}{Guid.NewGuid():N}"[..38];

            foreach (var product in products)
            {
                await using var updateStock = new SqlCommand(
                    "UPDATE dbo.Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductId = @ProductId AND StockQuantity >= @Quantity;",
                    connection, transaction);
                updateStock.Parameters.Add("@Quantity", SqlDbType.Int).Value = product.Quantity;
                updateStock.Parameters.Add("@ProductId", SqlDbType.Int).Value = product.ProductId;
                if (await updateStock.ExecuteNonQueryAsync() != 1)
                    throw new CheckoutRejectedException($"Tồn kho sản phẩm '{product.ProductName}' vừa thay đổi. Giao dịch đã bị hủy.");
            }

            int orderId;
            const string insertOrderSql = """
                INSERT INTO dbo.Orders (OrderCode, CreatedDate, CustomerId, CashierUsername, TotalItems, FinalAmount, RewardPoints)
                OUTPUT INSERTED.OrderId
                VALUES (@OrderCode, @CreatedDate, @CustomerId, @CashierUsername, @TotalItems, @FinalAmount, @RewardPoints);
                """;
            await using (var insertOrder = new SqlCommand(insertOrderSql, connection, transaction))
            {
                insertOrder.Parameters.Add("@OrderCode", SqlDbType.NVarChar, 50).Value = orderCode;
                insertOrder.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = DateTime.Now;
                insertOrder.Parameters.Add("@CustomerId", SqlDbType.Int).Value = (object?)request.CustomerId ?? DBNull.Value;
                insertOrder.Parameters.Add("@CashierUsername", SqlDbType.NVarChar, 50).Value = cashierUsername;
                insertOrder.Parameters.Add("@TotalItems", SqlDbType.Int).Value = totalItems;
                var amountParameter = insertOrder.Parameters.Add("@FinalAmount", SqlDbType.Decimal);
                amountParameter.Precision = 18; amountParameter.Scale = 2; amountParameter.Value = finalAmount;
                insertOrder.Parameters.Add("@RewardPoints", SqlDbType.Int).Value = rewardPoints;
                orderId = Convert.ToInt32(await insertOrder.ExecuteScalarAsync());
            }

            const string insertDetailSql = """
                INSERT INTO dbo.OrderDetails (OrderId, ProductId, Quantity, UnitPrice, LineTotal)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @LineTotal);
                """;
            foreach (var product in products)
            {
                await using var insertDetail = new SqlCommand(insertDetailSql, connection, transaction);
                insertDetail.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                insertDetail.Parameters.Add("@ProductId", SqlDbType.Int).Value = product.ProductId;
                insertDetail.Parameters.Add("@Quantity", SqlDbType.Int).Value = product.Quantity;
                var unitPrice = insertDetail.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                unitPrice.Precision = 18; unitPrice.Scale = 2; unitPrice.Value = product.Price;
                var lineTotal = insertDetail.Parameters.Add("@LineTotal", SqlDbType.Decimal);
                lineTotal.Precision = 18; lineTotal.Scale = 2; lineTotal.Value = product.Price * product.Quantity;
                await insertDetail.ExecuteNonQueryAsync();
            }

            int? customerPointsAfter = null;
            if (request.CustomerId.HasValue)
            {
                await using var addPoints = new SqlCommand(
                    "UPDATE dbo.Customers SET RewardPoints = RewardPoints + @Points WHERE CustomerId = @CustomerId; SELECT RewardPoints FROM dbo.Customers WHERE CustomerId = @CustomerId;",
                    connection, transaction);
                addPoints.Parameters.Add("@Points", SqlDbType.Int).Value = rewardPoints;
                addPoints.Parameters.Add("@CustomerId", SqlDbType.Int).Value = request.CustomerId.Value;
                customerPointsAfter = Convert.ToInt32(await addPoints.ExecuteScalarAsync());
            }

            await transaction.CommitAsync();
            return Ok(new { success = true, orderId, orderCode, finalAmount, totalItems, rewardPoints, customerRewardPoints = customerPointsAfter });
        }
        catch (CheckoutRejectedException ex)
        {
            await RollbackQuietlyAsync(transaction);
            return BadRequest(new { success = false, message = "Giao dịch thanh toán bị hủy và đã được Rollback an toàn!", errorDetail = ex.Message });
        }
        catch
        {
            await RollbackQuietlyAsync(transaction);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = "Không thể lưu hóa đơn. Mọi thay đổi trong giao dịch đã được hoàn tác." });
        }
    }

    // Bài tập mở rộng: hủy hóa đơn và hoàn kho/điểm bằng một transaction riêng.
    [HttpDelete("cancel/{orderId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            int? customerId;
            int points;
            await using (var orderCommand = new SqlCommand(
                "SELECT CustomerId, RewardPoints FROM dbo.Orders WITH (UPDLOCK, ROWLOCK) WHERE OrderId = @OrderId;",
                connection, transaction))
            {
                orderCommand.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                await using var reader = await orderCommand.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (!await reader.ReadAsync()) throw new CheckoutRejectedException("Không tìm thấy hóa đơn cần hủy.");
                customerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId"));
                points = reader.GetInt32(reader.GetOrdinal("RewardPoints"));
            }

            var details = new List<(int ProductId, int Quantity)>();
            await using (var detailsCommand = new SqlCommand(
                "SELECT ProductId, Quantity FROM dbo.OrderDetails WITH (UPDLOCK, ROWLOCK) WHERE OrderId = @OrderId;",
                connection, transaction))
            {
                detailsCommand.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                await using var reader = await detailsCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync()) details.Add((reader.GetInt32(0), reader.GetInt32(1)));
            }

            foreach (var line in details)
            {
                await using var restoreStock = new SqlCommand(
                    "UPDATE dbo.Products SET StockQuantity = StockQuantity + @Quantity WHERE ProductId = @ProductId;",
                    connection, transaction);
                restoreStock.Parameters.Add("@Quantity", SqlDbType.Int).Value = line.Quantity;
                restoreStock.Parameters.Add("@ProductId", SqlDbType.Int).Value = line.ProductId;
                if (await restoreStock.ExecuteNonQueryAsync() != 1)
                    throw new InvalidOperationException("Không thể hoàn tồn kho cho sản phẩm của hóa đơn.");
            }

            if (customerId.HasValue && points > 0)
            {
                await using var reversePoints = new SqlCommand(
                    "UPDATE dbo.Customers SET RewardPoints = CASE WHEN RewardPoints >= @Points THEN RewardPoints - @Points ELSE 0 END WHERE CustomerId = @CustomerId;",
                    connection, transaction);
                reversePoints.Parameters.Add("@Points", SqlDbType.Int).Value = points;
                reversePoints.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId.Value;
                await reversePoints.ExecuteNonQueryAsync();
            }

            await using (var deleteDetails = new SqlCommand("DELETE FROM dbo.OrderDetails WHERE OrderId = @OrderId;", connection, transaction))
            {
                deleteDetails.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                await deleteDetails.ExecuteNonQueryAsync();
            }
            await using (var deleteOrder = new SqlCommand("DELETE FROM dbo.Orders WHERE OrderId = @OrderId;", connection, transaction))
            {
                deleteOrder.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                if (await deleteOrder.ExecuteNonQueryAsync() != 1)
                    throw new InvalidOperationException("Không thể xóa hóa đơn.");
            }

            await transaction.CommitAsync();
            return Ok(new { success = true, message = "Đã hủy hóa đơn, hoàn tồn kho và điều chỉnh điểm thưởng.", orderId });
        }
        catch (CheckoutRejectedException ex)
        {
            await RollbackQuietlyAsync(transaction);
            return NotFound(new { success = false, message = ex.Message });
        }
        catch
        {
            await RollbackQuietlyAsync(transaction);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = "Không thể hủy hóa đơn. Mọi thay đổi đã được hoàn tác." });
        }
    }

    private static async Task<string?> GetActiveUserRoleAsync(SqlConnection connection, SqlTransaction transaction, string username)
    {
        const string sql = "SELECT Role, IsActive FROM dbo.Users WITH (UPDLOCK, ROWLOCK) WHERE Username = @Username;";
        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        return await reader.ReadAsync() && reader.GetBoolean(reader.GetOrdinal("IsActive"))
            ? reader.GetString(reader.GetOrdinal("Role"))
            : null;
    }

    private static async Task<int?> GetCustomerPointsAsync(SqlConnection connection, SqlTransaction transaction, int customerId)
    {
        await using var command = new SqlCommand(
            "SELECT RewardPoints FROM dbo.Customers WITH (UPDLOCK, ROWLOCK) WHERE CustomerId = @CustomerId;",
            connection, transaction);
        command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
        var result = await command.ExecuteScalarAsync();
        return result is null or DBNull ? null : Convert.ToInt32(result);
    }

    private static async Task<CheckoutProduct?> GetProductAsync(SqlConnection connection, SqlTransaction transaction, int productId)
    {
        const string sql = "SELECT ProductId, ProductName, Price, StockQuantity FROM dbo.Products WITH (UPDLOCK, ROWLOCK) WHERE ProductId = @ProductId;";
        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await reader.ReadAsync()) return null;
        return new CheckoutProduct
        {
            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity"))
        };
    }

    private static async Task RollbackQuietlyAsync(SqlTransaction transaction)
    {
        try { await transaction.RollbackAsync(); }
        catch (InvalidOperationException) { /* transaction already completed */ }
        catch (SqlException) { /* preserve the original failure response */ }
    }

    private sealed class CheckoutProduct
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int StockQuantity { get; init; }
        public int Quantity { get; set; }
    }

    private sealed class CheckoutRejectedException(string message) : Exception(message) { }
}
