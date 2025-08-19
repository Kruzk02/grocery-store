﻿using API.Dtos;
using API.Entity;

namespace API.Services;

/// <summary>
/// Defines operations for managing categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Asynchronously retrieves all categories from the database.
    /// </summary>
    /// <remarks>
    /// This implementation uses Entity Framework Core to query the
    /// <c>Category</c> table. Result are wrapped in a
    /// <see cref="ServiceResult{T}"/> object.
    /// </remarks>
    /// <returns>
    /// A task containing a <see cref="ServiceResult{T}"/> with
    /// a list of categories if successful; otherwise error details.
    /// </returns>
    Task<ServiceResult<List<Category>>> FindAll();
}