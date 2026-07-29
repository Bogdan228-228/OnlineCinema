using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IPlatformRepository
    {
        Task<Platform> AddPlatformAsync(Platform platform);
        Task<Platform> DeletePlatformAsync(int platformId);
        Task<Platform> EditPlatformAsync(Platform platform);
        Task<List<Platform>> GetAllPlatformsAsync();
        Task<Platform?> GetPlatformByIdAsync(int id);
        Task<Platform?> GetPlatformByNameAsync(string name);
    }
}