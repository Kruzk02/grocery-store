using System.Net;
using System.Net.Http.Json;

using Application.Dtos.Request;
using Application.Dtos.Response;

namespace Tests.Controller;

[TestFixture]
public class InvoiceControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task CreateReturnCreated()
    {
        var content = JsonContent.Create(new InvoiceDto(2));
        HttpResponseMessage response = await Client.PostAsync("/invoice", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var invoice = await response.Content.ReadFromJsonAsync<InvoiceResponse>();
        Assert.That(invoice, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.Id, Is.GreaterThan(0));
            Assert.That(invoice.OrderId, Is.EqualTo(2));
            Assert.That(invoice.InvoiceNumber, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test, Order(0)]
    public async Task CreateReturnNotFound()
    {
        var content = JsonContent.Create(new InvoiceDto(1));
        HttpResponseMessage response = await Client.PostAsync("/invoice", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(1)]
    public async Task FindByIdReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/invoice/1");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var invoice = await response.Content.ReadFromJsonAsync<InvoiceResponse>();
        Assert.That(invoice, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invoice.Id, Is.GreaterThan(0));
            Assert.That(invoice.OrderId, Is.EqualTo(2));
            Assert.That(invoice.InvoiceNumber, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test, Order(1)]
    public async Task FindByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/invoice/123");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

}
