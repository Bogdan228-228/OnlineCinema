using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public PlatformRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Platform> AddPlatformAsync(Platform platform)
        {
            _db.Platforms.Add(platform);
            await _db.SaveChangesAsync();
            return platform;
        }

        public async Task<Platform> EditPlatformAsync(Platform platform)
        {
            _db.Platforms.Update(platform);
            await _db.SaveChangesAsync();
            return platform;
        }

        public async Task<Platform> DeletePlatformAsync(int platformId)
        {
            var platform = await _db.Platforms.FindAsync(platformId);
            if (platform != null)
            {
                _db.Platforms.Remove(platform);
                await _db.SaveChangesAsync();
                return platform;
            }
            return new Platform();
        }

        public async Task<Platform?> GetPlatformByIdAsync(int id)
        {
            return await _db.Platforms.FindAsync(id);
        }

        public async Task<List<Platform>> GetAllPlatformsAsync()
        {
            return await _db.Platforms.ToListAsync();
        }

        public async Task<Platform?> GetPlatformByNameAsync(string name)
        {
            return await _db.Platforms.FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}
