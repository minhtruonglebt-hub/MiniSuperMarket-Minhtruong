using System.ComponentModel.DataAnnotations;

namespace MiniSuperMarket.API.Models;

public sealed class CheckoutRequestDto
{
    // Admin có thể chỉ định thu ngân để tiện kiểm thử; Cashier luôn được gán theo JWT.
    [StringLength(50)]
    public string? CashierUsername { get; set; }

    public int? CustomerId { get; set; }

    [Required, MinLength(1)]
    public List<CheckoutItemRequestDto> Items { get; set; } = new();
}

public sealed class CheckoutItemRequestDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 9999)]
    public int Quantity { get; set; }
}
