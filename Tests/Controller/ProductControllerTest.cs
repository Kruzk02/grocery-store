using System.Net;

using Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Tests.Repository;

namespace Tests.Controller;

[TestFixture]
public class ProductControllerTest
{
    private PostgresFixture _db;
    private HttpClient _client;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _db = new PostgresFixture();
        await _db.StartAsync();

        var connectionString = _db.GetConnectionString();

        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    ServiceDescriptor? descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseNpgsql(connectionString);
                    });

                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                });
            });

        using (IServiceScope scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
        }

        _client = factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _db.StopAsync();
        _client.Dispose();
    }

    [Test, Order(0)]
    public async Task CreateProductReturnsCreated()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Name"), "Name");
        content.Add(new StringContent("hello_world"), "Description");
        content.Add(new StringContent("12.99"), "Price");
        content.Add(new StringContent("1"), "CategoryId");
        content.Add(new StringContent("1"), "Quantity");

        HttpResponseMessage response = await _client.PostAsync("/product", content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }
    }

    [Test, Order(1)]
    public async Task FindProductByIdReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/product/1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test, Order(3)]
    public async Task FindProductsReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/product?Name=Name&Skip=0&SortBy=Price&Ascending=True&Take=10");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test, Order(4)]
    public async Task GetImageReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync("/product/nope.jpg");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

    [Test, Order(5)]
    public async Task FindOrderItemByIdReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/product/1/ordersItem");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test, Order(6)]
    public async Task UpdateReturnsOk()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Name"), "Name");
        content.Add(new StringContent("hello_world"), "Description");
        content.Add(new StringContent("12.99"), "Price");
        content.Add(new StringContent("1"), "CategoryId");
        content.Add(new StringContent("1"), "Quantity");

        HttpResponseMessage response = await _client.PutAsync("/product/1", content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test, Order(7)]
    public async Task DeleteByIdReturnsNoContent()
    {
        HttpResponseMessage response = await _client.DeleteAsync("/product/1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
