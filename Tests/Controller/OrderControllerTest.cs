using System.Net;
using System.Net.Http.Json;

using Application.Dtos.Request;
using Application.Dtos.Response;

namespace Tests.Controller;

[TestFixture]
public class OrderControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task CreateReturnCreated()
    {

        var content = JsonContent.Create(new OrderDto(2));
        HttpResponseMessage response = await Client.PostAsync("/order", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.That(order, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order.Id, Is.GreaterThan(0));
            Assert.That(order.CustomerId, Is.GreaterThan(0));
        }
    }

    [Test, Order(0)]
    public async Task CreateReturnNotFound()
    {
        var content = JsonContent.Create(new OrderDto(123));
        HttpResponseMessage response = await Client.PostAsync("/order", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(1)]
    public async Task FindByIdReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/order/1");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.That(order, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order.Id, Is.GreaterThan(0));
            Assert.That(order.CustomerId, Is.GreaterThan(0));
        }
    }

    [Test, Order(1)]
    public async Task FindByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/order/123");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(2)]
    public async Task FindOrderItemByIdReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/order/1/ordersItem");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test, Order(3)]
    public async Task FindInvoiceByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/order/1/invoice");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(4)]
    public async Task UpdateReturnOk()
    {
        var content = JsonContent.Create(new OrderDto(2));
        HttpResponseMessage response = await Client.PutAsync("/order/1", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.That(order, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order.Id, Is.GreaterThan(0));
            Assert.That(order.CustomerId, Is.GreaterThan(0));
        }
    }

    [Test, Order(5)]
    public async Task DeleteReturnNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/order/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(5)]
    public async Task DeleteReturnNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/order/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
