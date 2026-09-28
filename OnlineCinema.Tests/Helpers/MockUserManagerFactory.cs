using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Tests.Helpers;

public static class MockUserManagerFactory
{
    public static Mock<UserManager<User>> Create()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
