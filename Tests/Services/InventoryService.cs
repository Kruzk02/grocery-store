using Application.Common;
using Application.Dtos.Request;
using Application.Interface;
using Application.Repository;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace Tests.Services;

[TestFixture]
public class InventoryServiceTest
{
    private IInventoryService _inventoryService;
    private Mock<IInventoryRepository> _mockInventoryRepository;
    private Mock<IProductRepository> _mockProductRepository;

    [SetUp]
    public void Setup()
    {
        _mockInventoryRepository = new Mock<IInventoryRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _inventoryService = new InventoryService(_mockInventoryRepository.Object, _mockProductRepository.Object,
            new MemoryCache(new MemoryCacheOptions()));
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task CreateInventoryShouldCreateProduct(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(It.IsAny<int>())).ReturnsAsync(product);
        _mockInventoryRepository
            .Setup(x => x.Add(It.IsAny<Inventory>()))
            .ReturnsAsync((Inventory inv) => inv);
        Inventory result = await _inventoryService.Create(new InventoryDto(product.Id, 20));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ProductId, Is.EqualTo(product.Id));
            Assert.That(result.Product, Is.EqualTo(product));
            Assert.That(result.Stock, Is.EqualTo(20));
        }
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 2)]
    [TestCase(1, 3)]
    [TestCase(1, 4)]
    public Task CreateInventoryShouldThrowNotFoundException(int productId, int quantity)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _inventoryService.Create(new InventoryDto(productId, quantity)));

        Assert.That(ex.Message, Is.EqualTo($"Product with id: {productId} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task UpdateInventoryShouldUpdateInventory(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(product.Id)).ReturnsAsync(product);
        _mockInventoryRepository.Setup(x => x.Add(It.IsAny<Inventory>())).ReturnsAsync((Inventory i) => i);
        Inventory inventory = await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Stock: 20));

        _mockInventoryRepository.Setup(x => x.FindById(1)).ReturnsAsync(new Inventory
        {
            Product = product
        });
        _mockInventoryRepository.Setup(x => x.Update(It.IsAny<Inventory>()));
        Inventory result = await _inventoryService.Update(1, new InventoryDto(ProductId: product.Id, Stock: 10));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(inventory.Id));
            Assert.That(result.ProductId, Is.EqualTo(inventory.ProductId));
            Assert.That(result.Product, Is.EqualTo(inventory.Product));
            Assert.That(result.Stock, Is.EqualTo(10));
        }
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 2)]
    [TestCase(1, 3)]
    [TestCase(1, 4)]
    public Task UpdateInventoryShouldThrowNotFoundExceptionWhenInventoryNotFound(int productId, int quantity)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _inventoryService.Update(1, new InventoryDto(productId, quantity)));

        Assert.That(ex.Message, Is.EqualTo("Inventory with id: 1 not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task FindAllShouldReturnListOfInventory(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(product.Id)).ReturnsAsync(product);
        _mockInventoryRepository.Setup(x => x.Add(It.IsAny<Inventory>())).ReturnsAsync((Inventory i) => i);
        Inventory inv = await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Stock: 20));

        _mockInventoryRepository.Setup(x => x.FindAll(null, null, null, 0, 10))
            .ReturnsAsync(new PageResult<Inventory>(1, [inv]));
        PageResult<Inventory> result = await _inventoryService.FindAll(null, null, null, 0, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Has.Count.EqualTo(1));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task FindByIdShouldReturnInventory(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(product.Id)).ReturnsAsync(product);
        _mockInventoryRepository.Setup(x => x.Add(It.IsAny<Inventory>())).ReturnsAsync((Inventory i) => i);
        Inventory inventory = await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Stock: 20));

        _mockInventoryRepository.Setup(x => x.FindById(inventory.Id)).ReturnsAsync(inventory);
        Inventory result = await _inventoryService.FindById(inventory.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(inventory.Id));
            Assert.That(result.ProductId, Is.EqualTo(inventory.ProductId));
            Assert.That(result.Product, Is.EqualTo(inventory.Product));
            Assert.That(result.Stock, Is.EqualTo(inventory.Stock));
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
            await _inventoryService.FindById(id));

        Assert.That(ex.Message, Is.EqualTo($"Inventory with id: {id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task FindByProductIdShouldReturnListOfInventory(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(product.Id)).ReturnsAsync(product);
        _mockInventoryRepository.Setup(x => x.Add(It.IsAny<Inventory>())).ReturnsAsync((Inventory i) => i);
        Inventory inventory = await _inventoryService.Create(new InventoryDto(product.Id, 20));

        _mockInventoryRepository.Setup(x => x.FindAll(product.Id, 0, null, 0, 10))
            .ReturnsAsync(new PageResult<Inventory>(1, [inventory]));
        PageResult<Inventory> result = await _inventoryService.FindAll(product.Id, 0, null, 0, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(1));
            Assert.That(result.Data[0].Id, Is.EqualTo(inventory.Id));
            Assert.That(result.Data[0].ProductId, Is.EqualTo(inventory.ProductId));
            Assert.That(result.Data[0].Product, Is.EqualTo(inventory.Product));
            Assert.That(result.Data[0].Stock, Is.EqualTo(inventory.Stock));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task FindByQuantityShouldReturnListOfInventory(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(product.Id)).ReturnsAsync(product);
        _mockInventoryRepository.Setup(x => x.Add(It.IsAny<Inventory>())).ReturnsAsync((Inventory i) => i);
        Inventory inventory = await _inventoryService.Create(new InventoryDto(product.Id, 20));

        _mockInventoryRepository.Setup(x => x.FindAll(product.Id, 20, null, 0, 10))
            .ReturnsAsync(new PageResult<Inventory>(1, [inventory]));
        PageResult<Inventory> result = await _inventoryService.FindAll(product.Id, 20, null, 0, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(1));
            Assert.That(result.Data[0].Id, Is.EqualTo(inventory.Id));
            Assert.That(result.Data[0].ProductId, Is.EqualTo(inventory.ProductId));
            Assert.That(result.Data[0].Product, Is.EqualTo(inventory.Product));
            Assert.That(result.Data[0].Stock, Is.EqualTo(inventory.Stock));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateProduct))]
    public async Task DeleteByIdShouldDeleteInventory(Product product)
    {
        _mockProductRepository.Setup(x => x.FindById(product.Id)).ReturnsAsync(product);
        _mockInventoryRepository.Setup(x => x.Add(It.IsAny<Inventory>())).ReturnsAsync((Inventory i) => i);
        Inventory inventory = await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Stock: 20));

        _mockInventoryRepository.Setup(x => x.FindById(inventory.Id)).ReturnsAsync(inventory);
        var result = await _inventoryService.Delete(inventory.Id);
        Assert.That(result, Is.EqualTo("Inventory deleted successfully"));
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task DeleteByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _inventoryService.Delete(id));

        Assert.That(ex.Message, Is.EqualTo($"Inventory with id: {id} not found"));
        return Task.CompletedTask;
    }

    private static IEnumerable<Product> CreateProduct()
    {
        yield return new Product
        {
            Id = 1,
            Name = "name1",
            Description = "description",
            Price = 49.99m,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        yield return new Product
        {
            Id = 2,
            Name = "name2",
            Description = "description",
            Price = 99.99m,
            CategoryId = 10,
            Category = new Category
                { Id = 10, Name = "Household & Cleaning", Description = "Detergents, cleaning items" }
        };
        yield return new Product
        {
            Id = 3,
            Name = "name2",
            Description = "description",
            Price = 599.99m,
            CategoryId = 13,
            Category = new Category { Id = 13, Name = "Miscellaneous", Description = "Other / seasonal products" }
        };
    }
}
