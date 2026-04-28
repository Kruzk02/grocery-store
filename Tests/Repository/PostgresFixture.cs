using Testcontainers.PostgreSql;

namespace Tests.Repository;


public class PostgresFixture
{
    private PostgreSqlContainer Container { get; set; } = null!;

    public async Task StartAsync()
    {
        Container = new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, true)
            .Build();

        await Container.StartAsync();
    }

    public async Task StopAsync()
    {
        await Container.DisposeAsync();
    }

    public string GetConnectionString()
        => Container.GetConnectionString();
}
