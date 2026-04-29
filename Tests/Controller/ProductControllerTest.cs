using System.Net;
using System.Net.Http.Json;

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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }

    [Test, Order(1)]
    public async Task FindProductByIdReturnsOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test, Order(1)]
    public async Task FindProductByIdReturnsNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/1123");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }


    [Test, Order(3)]
    public async Task FindProductsReturnsOk()
    {
        HttpResponseMessage response =
            await Client.GetAsync("/product?Name=Name&Skip=0&SortBy=Price&Ascending=True&Take=10");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test, Order(4)]
    public async Task GetImageReturnsNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/nope.jpg");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

    [Test, Order(5)]
    public async Task FindOrderItemByIdReturnsOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/product/1/ordersItem");

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

        HttpResponseMessage response = await Client.PutAsync("/product/1", content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }

    [Test, Order(7)]
    public async Task DeleteByIdReturnsNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/product/1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }

    [Test, Order(7)]
    public async Task DeleteByIdReturnsNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/product/1234");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
