using System.Net;
using System.Net.Http.Json;

using Domain.Entity;

namespace Tests.Controller;

[TestFixture]
public class CategoryControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task GetAllReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/category");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.That(categories, Is.Not.Null.And.Not.Empty);

        foreach (Category category in categories)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(category, Is.Not.Null);
                Assert.That(category.Id, Is.GreaterThan(0));
                Assert.That(category.Name, Is.Not.Empty);
                Assert.That(category.Description, Is.Not.Empty);
            }
        }
    }
}
