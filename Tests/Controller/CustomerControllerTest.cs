using System.Net;
using System.Net.Http.Json;

using Application.Common;
using Application.Dtos.Request;

using Domain.Entity;

namespace Tests.Controller;

[TestFixture]
public class CustomerControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task CreateReturnCreated()
    {
        var content = JsonContent.Create(new CustomerDto("Name", "Email@gmail.com", "0123456789", "Address1245"));
        HttpResponseMessage response = await Client.PostAsync("/customer", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var customer = await response.Content.ReadFromJsonAsync<Customer>();
        Assert.That(customer, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("Name"));
            Assert.That(customer.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customer.Phone, Is.EqualTo("0123456789"));
            Assert.That(customer.Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(0)]
    public async Task CreateReturnBadRequest()
    {
        var content = JsonContent.Create("");
        HttpResponseMessage response = await Client.PostAsync("/customer", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(1)]
    public async Task FindAllReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

        Assert.That(customers, Is.Not.Null.And.Not.Empty);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customers[1].Id, Is.GreaterThan(0));
            Assert.That(customers[1].Name, Is.EqualTo("Name"));
            Assert.That(customers[1].Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customers[1].Phone, Is.EqualTo("0123456789"));
            Assert.That(customers[1].Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(2)]
    public async Task SearchReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/search?name=Na&skip=0&take=10");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var pageResult = await response.Content.ReadFromJsonAsync<PageResult<Customer>>();

        Assert.That(pageResult, Is.Not.Null);
        Assert.That(pageResult.Total, Is.GreaterThan(0));

        List<Customer> customers = pageResult.Data;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customers[0].Id, Is.GreaterThan(0));
            Assert.That(customers[0].Name, Is.EqualTo("Name"));
            Assert.That(customers[0].Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customers[0].Phone, Is.EqualTo("0123456789"));
            Assert.That(customers[0].Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(3)]
    public async Task GetOrderReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/1/orders");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

        Assert.That(customers, Is.Not.Null);
    }

    [Test, Order(4)]
    public async Task FindByIdReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var customer = await response.Content.ReadFromJsonAsync<Customer>();

        Assert.That(customer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("Name"));
            Assert.That(customer.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customer.Phone, Is.EqualTo("0123456789"));
            Assert.That(customer.Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(4)]
    public async Task FindByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(5)]
    public async Task FindByNameReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/Name/name");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var customer = await response.Content.ReadFromJsonAsync<Customer>();

        Assert.That(customer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("Name"));
            Assert.That(customer.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customer.Phone, Is.EqualTo("0123456789"));
            Assert.That(customer.Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(5)]
    public async Task FindByNameReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/123asdasd/name");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(6)]
    public async Task FindByEmailReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/Email@gmail.com/email");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var customer = await response.Content.ReadFromJsonAsync<Customer>();

        Assert.That(customer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("Name"));
            Assert.That(customer.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customer.Phone, Is.EqualTo("0123456789"));
            Assert.That(customer.Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(6)]
    public async Task FindByEmailReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/Email444444444444444@gmail.com/email");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(7)]
    public async Task FindByPhoneReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/0123456789/phone");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var customer = await response.Content.ReadFromJsonAsync<Customer>();

        Assert.That(customer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("Name"));
            Assert.That(customer.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(customer.Phone, Is.EqualTo("0123456789"));
            Assert.That(customer.Address, Is.EqualTo("Address1245"));
        }
    }

    [Test, Order(7)]
    public async Task FindByPhoneReturnNotFound()
    {
        HttpResponseMessage response = await Client.GetAsync("/customer/1123456789/phone");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(8)]
    public async Task UpdateReturnNoContent()
    {
        var content =
            JsonContent.Create(new CustomerDto("Name123", "Email123@gmail.com", "0123456789", "Address12345"));
        HttpResponseMessage response = await Client.PutAsync("/customer/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(8)]
    public async Task UpdateReturnNotFound()
    {
        var content =
            JsonContent.Create(new CustomerDto("Name123", "Email123@gmail.com", "0123456789", "Address12345"));
        HttpResponseMessage response = await Client.PutAsync("/customer/123", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(9)]
    public async Task DeleteByIdReturnNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/customer/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(9)]
    public async Task DeleteByIdReturnNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/customer/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
