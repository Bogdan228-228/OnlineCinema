using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OnlineCinema.DataAccess;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Tests.Fakes;

namespace OnlineCinema.Tests.Fixtures;

public class CinemaWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg = new PostgreSqlBuilder().WithImage("postgres:16-alpine").Build();
    private readonly RedisContainer _redis = new RedisBuilder().WithImage("redis:7-alpine").Build();
    private Respawner? _respawner;

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_pg.StartAsync(), _redis.StartAsync());

        Environment.SetEnvironmentVariable("DB_Connection", _pg.GetConnectionString());
        Environment.SetEnvironmentVariable("REDIS_Connection", _redis.GetConnectionString());
        Environment.SetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING", "UseDevelopmentStorage=true");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
        Environment.SetEnvironmentVariable("Jwt__AccessSecret", "test-access-secret-at-least-32-chars!!");
        Environment.SetEnvironmentVariable("Jwt__RefreshSecret", "test-refresh-secret-at-least-32-chars!");
        Environment.SetEnvironmentVariable("Jwt__AccessExpiresInMinutes", "60");
        Environment.SetEnvironmentVariable("Jwt__RefreshExpiresInDays", "7");

        _ = Services;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IMovieUploadService>();
            services.AddSingleton<FakeMovieUploadService>();
            services.AddSingleton<IMovieUploadService>(sp => sp.GetRequiredService<FakeMovieUploadService>());
            services.RemoveAll<IActorUploadService>();
            services.AddSingleton<FakeActorUploadService>();
            services.AddSingleton<IActorUploadService>(sp => sp.GetRequiredService<FakeActorUploadService>());
        });
    }

    public HttpClient CreateApiClient()
        => CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

    public async Task SeedAsync(Func<OnlineCinemaDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OnlineCinemaDbContext>();
        await action(db);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(_pg.GetConnectionString());
        await conn.OpenAsync();

        _respawner ??= await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" },
            TablesToIgnore = new Respawn.Graph.Table[]
            {
                "__EFMigrationsHistory", "SubscriptionPlans",
                "AspNetRoles", "AspNetUsers", "AspNetUserRoles"
            }
        });

        await _respawner.ResetAsync(conn);
    }

    public async Task<T> QueryAsync<T>(Func<OnlineCinemaDbContext, Task<T>> query)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OnlineCinemaDbContext>();
        return await query(db);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _pg.DisposeAsync();
        await _redis.DisposeAsync();
    }

}