using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniSuperMarket.API.Models;

[Table("Orders")]
public class Order
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }

    [Required, StringLength(50)]
    public string OrderCode { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public int? CustomerId { get; set; }

    [Required, StringLength(50)]
    public string CashierUsername { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalAmount { get; set; }

    public int RewardPoints { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
