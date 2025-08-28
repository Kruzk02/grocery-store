using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entity;

public class Inventory
{
    [Key]
    public int Id { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
    public int Quantity { get; set; } = 0;
    public DateTime UpdatedAt { get; set; }
}