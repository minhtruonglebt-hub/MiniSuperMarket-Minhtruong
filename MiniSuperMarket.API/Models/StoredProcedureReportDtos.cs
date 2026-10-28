namespace MiniSuperMarket.API.Models;

public sealed record CashierRevenueDto(
    string CashierUsername,
    int TotalOrders,
    decimal TotalRevenue,
    int TotalPointsAwarded);

public sealed record TopSellingProductDto(
    int ProductId,
    string ProductName,
    int QuantitySold,
    decimal Revenue);
