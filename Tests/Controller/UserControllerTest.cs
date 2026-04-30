using System.Net;
using System.Net.Http.Json;

using Application.Dtos.Request;
using Application.Dtos.Response;

using Domain.Entity;

namespace Tests.Controller;

[TestFixture]
public class UserControllerTest : BaseControllerTest
{
    [Test, Order(0)]
    public async Task RegisterReturnOk()
    {
        var content = JsonContent.Create(new RegisterDto("Username", "Email@gmail.com", "Password123!"));
        HttpResponseMessage response = await Client.PostAsync("/user/register", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.That(userResponse, Is.Not.Null);
        Assert.That(userResponse.Message, Is.Not.Null);
        Assert.That(userResponse.Message, Does.Contain("Success"));
    }

    [Test, Order(0)]
    public async Task RegisterReturnBadRequest()
    {
        HttpResponseMessage response = await Client.PostAsync("/user/register", JsonContent.Create(new {}));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(1)]
    public async Task LoginReturnOk()
    {
        var content = JsonContent.Create(new LoginDto("Username", "Password123!"));
        HttpResponseMessage response = await Client.PostAsync("/user/login", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        Assert.That(tokenResponse, Is.Not.Null);
        Assert.That(tokenResponse.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test, Order(1)]
    public async Task LoginReturnBadRequest()
    {
        HttpResponseMessage response = await Client.PostAsync("/user/login", JsonContent.Create(new {}));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(2)]
    public async Task GetUserReturnOk()
    {
        HttpResponseMessage response = await Client.GetAsync("/user?usernameOrEmail=Username");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.That(user, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(user.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(user.Email, Is.Not.Null.And.Not.Empty);
            Assert.That(user.Username, Is.Not.Null.And.Not.Empty);
            Assert.That(user.Roles, Is.Not.Empty);
        }
    }
}
