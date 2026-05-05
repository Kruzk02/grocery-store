using System.Net;
using System.Net.Http.Json;

using Domain.Entity;

namespace Tests.Controller;

[TestFixture]
public class NotificationControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task MarkAsReadReturnOk()
    {
        HttpResponseMessage response = await Client.PutAsync("/Notification/1", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await response.Content.ReadFromJsonAsync<Notification>();
        Assert.That(content, Is.Not.Null);
        Assert.That(content.IsRead, Is.True);
    }

    [Test, Order(0)]
    public async Task MarkAsReadReturnNotFound()
    {
        HttpResponseMessage response = await Client.PutAsync("/Notification/1124124", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Order(1)]
    public async Task MarkAllAsReadReturnOk()
    {
        HttpResponseMessage response = await Client.PutAsync("/Notification/all-as-read", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var notifications = await response.Content.ReadFromJsonAsync<List<Notification>>();
        Assert.That(notifications, Is.Not.Null.And.Not.Empty);

        foreach (Notification notification in notifications)
        {
            Assert.That(notification.IsRead, Is.True);
        }
    }

    [Test, Order(2)]
    public async Task DeleteNotificationReturnNoContent()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/Notification/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Order(2)]
    public async Task DeleteNotificationReturnNotFound()
    {
        HttpResponseMessage response = await Client.DeleteAsync("/Notification/123");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
