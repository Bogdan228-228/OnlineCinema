using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;

namespace OnlineCinema.Tests.Helpers;

public static class TestDbContextFactory
{
    public static OnlineCinemaDbContext Create()
    {
        var options = new DbContextOptionsBuilder<OnlineCinemaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OnlineCinemaDbContext(options);
    }
}
