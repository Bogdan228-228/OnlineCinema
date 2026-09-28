using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using OnlineCinema.DataAccess;
using OnlineCinema.DataAccess.Security;
using OnlineCinema.Domain.Constants;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Auth;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Services;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Services;

public class AuthServiceTests
{
    private static IConfiguration CreateConfiguration()
    {
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Jwt:RefreshExpiresInDays"]).Returns("30");
        return configurationMock.Object;
    }

    private static AuthService CreateService(
        Mock<UserManager<User>> userManagerMock,
        OnlineCinemaDbContext dbContext,
        Mock<IJwtTokenGenerator>? jwtMock = null,
        Mock<ITokenHasher>? hasherMock = null)
    {
        jwtMock ??= new Mock<IJwtTokenGenerator>();
        hasherMock ??= new Mock<ITokenHasher>();

        return new AuthService(
            userManagerMock.Object,
            dbContext,
            jwtMock.Object,
            hasherMock.Object,
            CreateConfiguration());
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyTaken_ThrowsConflictException()
    {
        var userManagerMock = MockUserManagerFactory.Create();
        userManagerMock
            .Setup(m => m.FindByEmailAsync("taken@test.com"))
            .ReturnsAsync(new User { Email = "taken@test.com" });

        var dbContext = TestDbContextFactory.Create();
        var service = CreateService(userManagerMock, dbContext);

        var request = new RegisterRequest { Email = "taken@test.com", Password = "Password1", FullName = "Test" };

        await Assert.ThrowsAsync<ConflictException>(() => service.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsFree_CreatesUserAndAssignsDefaultRole()
    {
        var userManagerMock = MockUserManagerFactory.Create();
        userManagerMock
            .Setup(m => m.FindByEmailAsync("new@test.com"))
            .ReturnsAsync((User?)null);
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), "Password1"))
            .Callback<User, string>((user, _) => user.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), RoleNames.User))
            .ReturnsAsync(IdentityResult.Success);

        var dbContext = TestDbContextFactory.Create();
        var service = CreateService(userManagerMock, dbContext);

        var request = new RegisterRequest { Email = "new@test.com", Password = "Password1", FullName = "Test" };

        var userId = await service.RegisterAsync(request);

        Assert.NotEqual(Guid.Empty, userId);
        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), RoleNames.User), Times.Once);
        Assert.Single(dbContext.EmailTokens);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorizedException()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "u@test.com", IsActive = true };

        var userManagerMock = MockUserManagerFactory.Create();
        userManagerMock.Setup(m => m.FindByEmailAsync("u@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        var dbContext = TestDbContextFactory.Create();
        var service = CreateService(userManagerMock, dbContext);

        var request = new LoginRequest { Email = "u@test.com", Password = "wrong" };

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request, "test-agent"));
    }

    [Fact]
    public async Task LoginAsync_WhenUserInactive_ThrowsUnauthorizedException()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "inactive@test.com", IsActive = false };

        var userManagerMock = MockUserManagerFactory.Create();
        userManagerMock.Setup(m => m.FindByEmailAsync("inactive@test.com")).ReturnsAsync(user);

        var dbContext = TestDbContextFactory.Create();
        var service = CreateService(userManagerMock, dbContext);

        var request = new LoginRequest { Email = "inactive@test.com", Password = "whatever" };

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request, "test-agent"));
        userManagerMock.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenPairAndStoresRefreshToken()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "u@test.com", IsActive = true };

        var userManagerMock = MockUserManagerFactory.Create();
        userManagerMock.Setup(m => m.FindByEmailAsync("u@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(m => m.CheckPasswordAsync(user, "Password1")).ReturnsAsync(true);
        userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { RoleNames.User });

        var jwtMock = new Mock<IJwtTokenGenerator>();
        jwtMock.Setup(j => j.GenerateAccessToken(user, It.IsAny<IEnumerable<string>>())).Returns("access-token");
        jwtMock.Setup(j => j.GenerateRefreshToken(user)).Returns("refresh-token");

        var hasherMock = new Mock<ITokenHasher>();
        hasherMock.Setup(h => h.Hash("refresh-token")).Returns("hashed-refresh-token");

        var dbContext = TestDbContextFactory.Create();
        var service = CreateService(userManagerMock, dbContext, jwtMock, hasherMock);

        var request = new LoginRequest { Email = "u@test.com", Password = "Password1" };

        var tokens = await service.LoginAsync(request, "test-agent");

        Assert.Equal("access-token", tokens.AccessToken);
        Assert.Equal("refresh-token", tokens.RefreshToken);
        Assert.Single(dbContext.RefreshTokens);
    }
}
