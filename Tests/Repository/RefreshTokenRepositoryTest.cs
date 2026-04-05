using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class RefreshTokenRepositoryTest
{

    private PostgresFixture _fixture;
    private string _connectionString;

    private RefreshToken _refreshToken;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateDbContext();
        await context.Database.MigrateAsync();

        _refreshToken = new RefreshToken()
        {
            Token = Guid.NewGuid().ToString(),
            UserId = "userId",
            IsRevoked = false,
        };

        context.RefreshTokens.Add(_refreshToken);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task AddShouldSave()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new RefreshTokenRepository(context);
        var refreshToken = new RefreshToken()
        {
            Token = Guid.NewGuid().ToString(),
            UserId = "userId123",
            IsRevoked = false,
        };
        await repository.Add(refreshToken);
        Assert.That(context.RefreshTokens.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task FindByTokenShouldReturnRefreshToken()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new RefreshTokenRepository(context);

        RefreshToken? refreshToken = await repository.FindByToken(_refreshToken.Token);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(refreshToken, Is.Not.Null);
            Assert.That(refreshToken.Id, Is.EqualTo(1));
            Assert.That(refreshToken.UserId, Is.EqualTo("userId"));
            Assert.That(refreshToken.IsRevoked, Is.False);
            Assert.That(refreshToken.Token, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByTokenShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new RefreshTokenRepository(context);

        RefreshToken? refreshToken = await repository.FindByToken("aoiwdbaoiwdbaoiwd");
        Assert.That(refreshToken, Is.Null);
    }

    [Test]
    public async Task RevokeTokenByUserIdShouldUpdateRefreshToken()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new RefreshTokenRepository(context);

        await repository.RevokeTokenByUserId(_refreshToken.UserId);
        RefreshToken? refreshToken = await context.RefreshTokens
            .Where(x => x.UserId == _refreshToken.UserId && x.IsRevoked).FirstOrDefaultAsync();
        Assert.That(refreshToken!.IsRevoked, Is.True);
    }

    [Test]
    public async Task DeleteAllByUserIdShouldDeleteAllRefreshTokens()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new RefreshTokenRepository(context);

        await repository.DeleteAllByUserId(_refreshToken.UserId);
        Assert.That(context.RefreshTokens.Count(), Is.Zero);
    }

    private ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString)
            .Options);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}
