using System.Net;
using System.Net.Http.Json;

using Application.Dtos.Request;

using Domain.Entity;

namespace Tests.Controller;

[TestFixture]
public class OrderItemControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task CreateReturnCreated()
    {
        var content = JsonContent.Create(new OrderItemDto(2, 2, 1));
        HttpResponseMessage response = await Client.PostAsync("/orderItem", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var orderItem = await response.Content.ReadFromJsonAsync<OrderItem>();
        Assert.That(orderItem, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(orderItem.Id, Is.GreaterThan(0));
            Assert.That(orderItem.ProductId, Is.GreaterThan(0));
            Assert.That(orderItem.OrderId, Is.GreaterThan(0));
            Assert.That(orderItem.Quantity, Is.GreaterThanOrEqualTo(1));
        }
    }

    [Test, Order(0)]
    public async Task CreateReturnBadRequest()
    {
        var content = JsonContent.Create(new OrderItemDto(2, 2, 120));
        HttpResponseMessage response = await Client.PostAsync("/orderItem", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(0)]
    public async Task CreateReturnNotFound()
    {
        var content = JsonContent.Create(new OrderItemDto(1, 1, 120));
        HttpResponseMessage response = await Client.PostAsync("/orderItem", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(1)]
    public async Task FindByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/orderItem/2");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(2)]
    public async Task UpdateReturnOk()
    {
        var content = JsonContent.Create(new OrderItemDto(2, 2, 0));
        HttpResponseMessage response = await Client.PutAsync("/orderItem/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));


        var orderItem = await response.Content.ReadFromJsonAsync<OrderItem>();
        Assert.That(orderItem, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(orderItem.Id, Is.GreaterThan(0));
            Assert.That(orderItem.ProductId, Is.GreaterThan(0));
            Assert.That(orderItem.OrderId, Is.GreaterThan(0));
            Assert.That(orderItem.Quantity, Is.Zero);
        }
    }

    [Test, Order(2)]
    public async Task UpdateReturnBadRequest()
    {
        var content = JsonContent.Create(new OrderItemDto(2, 2, 120));
        HttpResponseMessage response = await Client.PutAsync("/orderItem/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(2)]
    public async Task UpdateReturnNotFound()
    {
        var content = JsonContent.Create(new OrderItemDto(1, 1, 120));
        HttpResponseMessage response = await Client.PutAsync("/orderItem/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(3)]
    public async Task DeleteReturnNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/orderItem/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(3)]
    public async Task DeleteReturnNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/orderItem/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
