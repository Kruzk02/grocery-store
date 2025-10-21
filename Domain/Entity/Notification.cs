using System.ComponentModel.DataAnnotations;

namespace Domain.Entity;

public class Notification
{
    [Key]
    public int Id { get; set; } [Required]
    public string UserId { get; set; } 
    public NotificationType Type { get; set; } = NotificationType.Info;
    [Required, MaxLength(500)]
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

}

public enum NotificationType
{
    Info,
    Warning,
    Success,
    Error
}