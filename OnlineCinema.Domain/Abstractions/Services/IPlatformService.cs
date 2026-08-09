using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IPlatformService
    {
        Task<Platform> AddPlatformAsync(string name);
        Task<bool> DeletePlatformAsync(int platformId);
        Task<Platform?> EditPlatformAsync(int id, string name);
        Task<List<Platform>> GetAllPlatformsAsync();
        Task<Platform?> GetPlatformByIdAsync(int id);
        Task<Platform?> GetPlatformByNameAsync(string name);
    }
}
