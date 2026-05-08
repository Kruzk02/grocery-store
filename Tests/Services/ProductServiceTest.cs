using Application.Common;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interface;
using Application.Queries;
using Application.Repository;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Infrastructure.FileSystem;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace Tests.Services;

[TestFixture]
public class ProductServiceTest
{
    private IProductService _productService;
    private IImageStorage _imageService;
    private Mock<IProductRepository> _mockProductRepository;
    private Mock<ICategoryRepository> _mockCategoryRepository;

    private Category _category;
    private Product _product;
    [SetUp]
    public void SetUp()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _imageService = new FileSystemImageStorage("wwww");
        _productService = new ProductService(_mockProductRepository.Object, _mockCategoryRepository.Object, _imageService, new MemoryCache(new MemoryCacheOptions()));

        _category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        _product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 19.99m,
            CategoryId = 1,
            Quantity = 25,
            CreatedAt = DateTime.UtcNow,
            Category = _category
        };
    }
    [Test]
    [TestCaseSource(nameof(CreateProductDto))]
    public async Task CreateProductShouldCreateProduct(ProductDto productDto)
    {
        _mockCategoryRepository.Setup(x => x.FindById(productDto.CategoryId)).ReturnsAsync(_category);
        _mockProductRepository.Setup(x => x.Add(It.IsAny<Product>())).ReturnsAsync(_product);
        ProductResponse result = await _productService.Create(productDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Does.StartWith("name"));
            Assert.That(result.Description, Does.StartWith("description"));
            Assert.That(result.Price, Is.EqualTo(19.99m));
            Assert.That(result.CategoryId, Is.EqualTo(1));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateProductDto))]
    public Task CreateProductShouldThrowNotFoundException(ProductDto productDto)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _productService.Create(productDto));

        Assert.That(ex!.Message, Is.EqualTo($"Category with id {productDto.CategoryId} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateProductDto))]
    public async Task UpdateProductShouldUpdateProduct(ProductDto productDto)
    {
        _mockProductRepository.Setup(x => x.FindById(1)).ReturnsAsync(_product);

        ProductResponse result = await _productService.Update(_product.Id, new ProductDto(Name: "name123", Description: "description123", Price: 11.99m, CategoryId: 1, Quantity: 44, "image.jpg"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Does.StartWith("name"));
            Assert.That(result.Description, Does.StartWith("description"));
            Assert.That(result.CategoryId, Is.EqualTo(1));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateProductDto))]
    public Task UpdateProductShouldThrowNotFoundException(ProductDto productDto)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _productService.Update(1, productDto));

        Assert.That(ex!.Message, Is.EqualTo("Product with id 1 not found"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task SearchProductsShouldReturnListOfProduct()
    {
        _mockProductRepository.Setup(x =>
            x.Search(new SearchProductQuery(_product.Name, 0, ProductSortBy.Name, false))).ReturnsAsync(new PageResult<Product>(1, [_product]));
        PageResult<ProductResponse> result = await _productService.SearchProducts(new SearchProductQuery(_product.Name, 0, ProductSortBy.Name, false));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Total, Is.GreaterThan(0));
            Assert.That(result.Data, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnProduct()
    {
        _mockProductRepository.Setup(x => x.FindById(1)).ReturnsAsync(_product);
        ProductResponse result = await _productService.FindById(_product.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Does.StartWith("name"));
            Assert.That(result.Description, Does.StartWith("description"));
            Assert.That(result.CategoryId, Is.EqualTo(1));
        }
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task FindByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _productService.FindById(id));

        Assert.That(ex!.Message, Is.EqualTo($"Product with id {id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task DeleteById()
    {
        _mockProductRepository.Setup(x => x.FindById(1)).ReturnsAsync(_product);
        var result = await _productService.DeleteById(_product.Id);

        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task DeleteByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _productService.DeleteById(id));

        Assert.That(ex!.Message, Is.EqualTo($"Product with id {id} not found"));
        return Task.CompletedTask;
    }

    private static IEnumerable<ProductDto> CreateProductDto()
    {
        yield return new ProductDto(Name: "name123", Description: "description3", Price: 2.99m, CategoryId: 1, Quantity: 1, "image.jpg");
        yield return new ProductDto(Name: "name4", Description: "description", Price: 5.99m, CategoryId: 1, Quantity: 1, "image.jpg");
        yield return new ProductDto(Name: "name56", Description: "description", Price: 6.99m, CategoryId: 1, Quantity: 1, "image.jpg");
    }
}
