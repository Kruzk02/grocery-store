using System.Net;
using System.Net.Http.Json;

using Application.Common;
using Application.DTOs.Response;

namespace Tests.Controller;

[TestFixture]
public class ProductControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task CreateProductReturnsCreated()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Name"), "Name");
        content.Add(new StringContent("hello_world"), "Description");
        content.Add(new StringContent("12.99"), "Price");
        content.Add(new StringContent("1"), "CategoryId");
        content.Add(new StringContent("1"), "Quantity");

        HttpResponseMessage response = await Client.PostAsync("/product", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "A request should succeed created product.");

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.That(product, Is.Not.Null, "Response body should not be null");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.Id, Is.GreaterThan(0));
            Assert.That(product.Name, Is.EqualTo("Name"));
            Assert.That(product.Description, Is.EqualTo("hello_world"));
            Assert.That(product.Price, Is.EqualTo(12.99m));
            Assert.That(product.CategoryId, Is.EqualTo(1));
            Assert.That(product.Quantity, Is.EqualTo(1));
        }
    }

    [Test, Order(0)]
    public async Task CreateProductReturnsBadRequest()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("12.99"), "Price");
        content.Add(new StringContent("1"), "CategoryId");
        content.Add(new StringContent("1"), "Quantity");

        HttpResponseMessage response = await Client.PostAsync("/product", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(1)]
    public async Task FindProductByIdReturnsOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.That(product, Is.Not.Null, "Response body should not be null");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.Id, Is.GreaterThan(0));
            Assert.That(product.Name, Is.EqualTo("Name"));
            Assert.That(product.Description, Is.EqualTo("hello_world"));
            Assert.That(product.Price, Is.EqualTo(12.99m));
            Assert.That(product.CategoryId, Is.EqualTo(1));
            Assert.That(product.Quantity, Is.EqualTo(1));
        }
    }

    [Test, Order(1)]
    public async Task FindProductByIdReturnsNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/1123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(3)]
    public async Task FindProductsReturnsOk()
    {
        HttpResponseMessage response =
            await Client.GetAsync("/product?Name=Name&Skip=0&SortBy=Price&Ascending=True&Take=10");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var pageResult = await response.Content.ReadFromJsonAsync<PageResult<ProductResponse>>();
        Assert.That(pageResult, Is.Not.Null);
        Assert.That(pageResult.Total, Is.GreaterThan(0));

        IReadOnlyList<ProductResponse> products = pageResult.Data;
        Assert.That(products, Is.Not.Null.And.Not.Empty, "List of product should not be null and empty");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(products[0].Id, Is.GreaterThan(0));
            Assert.That(products[0].Name, Is.EqualTo("Name"));
            Assert.That(products[0].Description, Is.EqualTo("hello_world"));
            Assert.That(products[0].Price, Is.EqualTo(12.99m));
            Assert.That(products[0].CategoryId, Is.EqualTo(1));
            Assert.That(products[0].Quantity, Is.EqualTo(1));
        }
    }

    [Test, Order(4)]
    public async Task GetImageReturnsNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/nope.jpg");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(5)]
    public async Task FindOrderItemByIdReturnsOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/1/ordersItem");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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

        HttpResponseMessage response = await Client.PutAsync("/product/1", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.That(product, Is.Not.Null, "Response body should not be null");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.Id, Is.GreaterThan(0));
            Assert.That(product.Name, Is.EqualTo("Name"));
            Assert.That(product.Description, Is.EqualTo("hello_world"));
            Assert.That(product.Price, Is.EqualTo(12.99m));
            Assert.That(product.CategoryId, Is.EqualTo(1));
            Assert.That(product.Quantity, Is.EqualTo(1));
        }
    }

    [Test, Order(6)]
    public async Task UpdateReturnsBadRequest()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("12.99"), "Price");
        content.Add(new StringContent("1"), "CategoryId");
        content.Add(new StringContent("1"), "Quantity");

        HttpResponseMessage response = await Client.PutAsync("/product/1", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(7)]
    public async Task DeleteByIdReturnsNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/product/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(7)]
    public async Task DeleteByIdReturnsNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/product/1234");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
