using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<OrderItem> Items { get; set; } = [];

    public decimal Total => Items.Sum(i => i.Quantity * i.Product.Price);
}