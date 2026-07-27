using Microsoft.EntityFrameworkCore;

namespace OnlineCinema.DataAccess
{
    public class OnlineCinemaDbContext : DbContext
    {
        public OnlineCinemaDbContext(DbContextOptions<OnlineCinemaDbContext> options) : base(options)
        {
        }
    }
}
