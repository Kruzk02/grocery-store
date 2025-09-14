using API.Dtos;
using API.Entity;

namespace API.Services;

/// <summary>
/// Defines operations for managing notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Asynchronously create a new notification in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which contains either the created notification or error details.
    /// </remarks>
    /// <param name="notification">The <see cref="Notification"/> that provides notification data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult<Notification>> Create(Notification notification);
    /// <summary>
    /// Asynchronously retrieves a notification by user identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which contains either the notification or error details.
    /// </remarks>
    /// <param name="userId">The identifier of the user to retrieve</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult<List<Notification>>> FindByUserId(string userId);
    /// <summary>
    /// Asynchronously deletes a notification by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which indicates success or contains error details.
    /// </remarks>
    /// <param name="id">The identifier of the notification to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult> DeleteById(int id);
    /// <summary>
    /// Asynchronously mark a notification is read by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which indicates success or contains error details.
    /// </remarks>
    /// <param name="id">The identifier of the notification to mark as read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult> MarkAsRead(int id);
    /// <summary>
    /// Asynchronously mark all notifications is read by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which indicates success or contains error details.
    /// </remarks>
    /// <param name="userId">The identifier of the user own notifications to mark all read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult> MarkAllAsRead(string userId);
}