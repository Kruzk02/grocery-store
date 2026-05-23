using Domain.Entity;

namespace Application.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> FindAll();
    Task<Category?> FindById(int id);
}
