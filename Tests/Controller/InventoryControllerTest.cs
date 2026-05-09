using System.Net;
using System.Net.Http.Json;

using Application.Dtos.Request;
using Application.Dtos.Response;

namespace Tests.Controller;

[TestFixture]
public class InventoryControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task CreateReturnCreated()
    {
        var content = JsonContent.Create(new InventoryDto(2, 40));
        HttpResponseMessage response = await Client.PostAsync("/inventory", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var inventory = await response.Content.ReadFromJsonAsync<InventoryResponse>();
        Assert.That(inventory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(inventory.Id, Is.GreaterThan(0));
            Assert.That(inventory.ProductId, Is.EqualTo(2));
            Assert.That(inventory.Stock, Is.EqualTo(40));
        }
    }

    [Test, Order(0)]
    public async Task CreateReturnNotFound()
    {
        var content = JsonContent.Create(new InventoryDto(1, 40));
        HttpResponseMessage response = await Client.PostAsync("/inventory", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(1)]
    public async Task FindAllReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/inventory");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test, Order(2)]
    public async Task FindByIdReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/inventory/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var inventory = await response.Content.ReadFromJsonAsync<InventoryResponse>();
        Assert.That(inventory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(inventory.Id, Is.GreaterThan(0));
            Assert.That(inventory.ProductId, Is.EqualTo(2));
            Assert.That(inventory.Stock, Is.EqualTo(40));
        }
    }

    [Test, Order(2)]
    public async Task FindByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/inventory/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(3)]
    public async Task UpdateReturnNoContent()
    {
        var content = JsonContent.Create(new InventoryDto(2, 80));
        HttpResponseMessage response = await Client.PutAsync("/inventory/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(3)]
    public async Task UpdateReturnNotFound()
    {
        var content = JsonContent.Create(new InventoryDto(1, 80));
        HttpResponseMessage response = await Client.PutAsync("/inventory/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(3)]
    public async Task DeleteReturnNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/inventory/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(3)]
    public async Task DeleteReturnNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/inventory/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
