using Domain.Entity;

namespace Application.Interfaces;

/// <summary>
/// Defines operations for managing categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Asynchronously retrieves all categories from the database.
    /// </summary>
    /// <returns>
    /// a list of categories if successful; otherwise empty list.
    /// </returns>
    Task<List<Category>> FindAll();
}
