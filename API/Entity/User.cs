using System.ComponentModel.DataAnnotations;

namespace API.Entity;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }
    [Required, EmailAddress, StringLength(255)]
    public string Email { get; set; }
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}