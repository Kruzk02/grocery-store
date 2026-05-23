using Application.Common;
using Application.Queries;

using Domain.Entity;

namespace Application.Repositories;

public interface IProductRepository
{
    Task<PageResult<Product>> Search(SearchProductQuery searchProductQuery);
    Task<Product> Add(Product product);
    Task Update(Product product);
    Task<Product?> FindById(int id);
    Task Delete(Product product);
}
