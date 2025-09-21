using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entity;

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId)), JsonIgnore]
    public Order Order { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public decimal SubTotal => Quantity * Product.Price;
}