using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineCinema.Domain.Constants;
using OnlineCinema.Domain.Models;
using OnlineCinema.Tests.Fixtures;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OnlineCinema.Tests.Helpers;

public record RegisterResult(Guid UserId);
public record TokenResult(string AccessToken, string RefreshToken);

public static class AuthHelper
{
    public const string Password = "Test!12345";

    public static async Task<(HttpClient Client, Guid UserId)> CreateUserClientAsync(CinemaWebApplicationFactory factory)
    {
        var client = factory.CreateApiClient();
        var email = $"{Guid.NewGuid():N}@test.com";

        var register = await client.PostAsJsonAsync("/api/auth/register",
            new { email, password = Password, fullName = "Test User" });
        register.EnsureSuccessStatusCode();
        var userId = (await register.Content.ReadFromJsonAsync<RegisterResult>())!.UserId;

        await LoginAsync(client, email, Password);
        return (client, userId);
    }

    public static async Task<HttpClient> CreateAdminClientAsync(CinemaWebApplicationFactory factory)
    {
        var client = factory.CreateApiClient();
        var email = $"{Guid.NewGuid():N}@test.com";

        var register = await client.PostAsJsonAsync("/api/auth/register",
            new { email, password = Password, fullName = "Test Admin" });
        register.EnsureSuccessStatusCode();
        var userId = (await register.Content.ReadFromJsonAsync<RegisterResult>())!.UserId;

        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(userId.ToString());
            var result = await userManager.AddToRoleAsync(user!, RoleNames.Admin);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        await LoginAsync(client, email, Password);
        return client;
    }

    private static async Task LoginAsync(HttpClient client, string email, string password)
    {
        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenResult>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
    }
}