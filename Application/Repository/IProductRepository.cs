using Application.Common;
using Application.Queries;

using Domain.Entity;

namespace Application.Repository;

public interface IProductRepository
{
    Task<PageResult<Product>> Search(SearchProductQuery searchProductQuery);
    Task<Product> Add(Product product);
    Task Update(Product product);
    Task<Product?> FindById(int Id);
    Task Delete(Product product);
}
