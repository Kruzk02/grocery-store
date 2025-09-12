using API.Dtos;
using API.Entity;

namespace API.Services;

public interface INotificationService
{
    Task<ServiceResult<Notification>> Create(Notification notification);
    Task<ServiceResult<List<Notification>>> FindByUserId(string userId);
    Task<ServiceResult> DeleteById(int id);
    Task<ServiceResult> MarkAsRead(int id);
    Task<ServiceResult> MarkAllAsRead(string userId);
}