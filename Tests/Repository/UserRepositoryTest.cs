using Application.Repositories;

using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Repository;

[TestFixture]
public class UserRepositoryTest
{
    private PostgresFixture _fixture;
    private ServiceProvider _provider;

    private User _user;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();

        _provider = BuildServiceProvider(_fixture.GetConnectionString());
        await InitializeDatabase(_provider);

        _user = new User
        {
            Username = "testuser",
            Email = "test@mail.com"
        };
    }

    [Test]
    public async Task AddUserShouldCreateUserWithRole()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User result = await repo.Add(_user, "Password123!");
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);

        IList<string> roles = await repo.GetRoles(result);
        Assert.That(roles, Is.Not.Null);
    }

    [Test]
    public async Task FindByIdShouldReturnUser()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");

        User? result = await repo.FindById(savedUser.Id!);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
    }

    [Test]
    public Task FindByIdShouldReturnNull()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var result = Assert.ThrowsAsync<Exception>(async () => await repo.FindById("InvalidId"));
        Assert.That(result.Message, Is.EqualTo("User not found with id: InvalidId"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task FindByUsernameShouldReturnUser()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");

        User? result = await repo.FindByUsername(savedUser.Username!);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
    }

    [Test]
    public Task FindByUsernameShouldReturnNull()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var result = Assert.ThrowsAsync<Exception>(async () => await repo.FindByUsername("123123"));
        Assert.That(result.Message, Is.EqualTo("User not found with username: 123123"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task FindByEmailShouldReturnUser()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");

        User? result = await repo.FindByEmail(savedUser.Email!);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
    }

    [Test]
    public Task FindByEmailShouldReturnNull()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var result = Assert.ThrowsAsync<Exception>(async () => await repo.FindByEmail("awd@gmail.com"));
        Assert.That(result.Message, Is.EqualTo("User not found with email: awd@gmail.com"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task GetRolesShouldReturnListOfString()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");

        IList<string> result = await repo.GetRoles(savedUser);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetUserByRoleShouldReturnListOfuser()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        await repo.Add(_user, "Password123!");

        List<User> result = await repo.GetUserByRole("Employee", CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CheckPasswordSignInReturnsTrue()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.CheckPasswordSignIn(savedUser, "Password123!");
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckPasswordSignInReturnsFalse()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.CheckPasswordSignIn(savedUser, "Password");
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateEmailReturnsTrue()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.UpdateEmail(savedUser, "test123@gmail.com");
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task UpdateUsernameReturnsTrue()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.UpdateUsername(savedUser, "testUser123");
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task UpdatePasswordReturnsTrue()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.UpdatePassword(savedUser, "Password123456!");
        Assert.That(result, Is.True);
    }


    [Test]
    public async Task UpdateRolesReturnsTrue()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.UpdateRoles(savedUser, "Manager");
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task DeleteShouldDelete()
    {
        using IServiceScope scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User savedUser = await repo.Add(_user, "Password123!");
        var result = await repo.Delete(savedUser);
        Assert.That(result, Is.True);
    }

    private static ServiceProvider BuildServiceProvider(string connectionString)
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddLogging();

        services.AddScoped<IUserRepository, UserRepository>();

        return services.BuildServiceProvider();
    }

    private static async Task InitializeDatabase(IServiceProvider provider)
    {
        using IServiceScope scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Admin", "Manager", "Employee"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}
