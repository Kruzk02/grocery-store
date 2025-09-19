using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entity;

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer customer { get; set; }
    [Required, Range(0.01, double.MaxValue, ErrorMessage = "Total must be greater than 0")]
    public double Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}