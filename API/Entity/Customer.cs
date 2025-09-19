using System.ComponentModel.DataAnnotations;

namespace API.Entity;

public class Customer
{
    [Key]
    public int Id { get; set; }
    [Required, MinLength(3), MaxLength(255)]
    public required string Name { get; set; }
    [Required, EmailAddress, MinLength(3), MaxLength(255)]
    public required string Email { get; set; }
    [Required, Phone]
    public required string Phone { get; set; }
    [Required, MinLength(3), MaxLength(255)]
    public required string Address { get; set; }
    
    public ICollection<Order> Orders { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}