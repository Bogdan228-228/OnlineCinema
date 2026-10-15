using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IPlatformRepository _platformRepository;

        public PlatformService(IPlatformRepository platformRepository)
        {
            _platformRepository = platformRepository;
        }

        public async Task<Platform> AddPlatformAsync(string name)
        {
            var platform = new Platform
            {
                Name = name
            };
            return await _platformRepository.AddPlatformAsync(platform);
        }

        public async Task<Platform?> EditPlatformAsync(int id, string name)
        {
            var platform = await _platformRepository.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                return null;
            }
            platform.Name = name;
            return await _platformRepository.EditPlatformAsync(platform);
        }

        public async Task<bool> DeletePlatformAsync(int platformId)
        {
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform != null)
            {
                return await _platformRepository.DeletePlatformAsync(platformId);
            }
            return false;
        }

        public async Task<Platform?> GetPlatformByIdAsync(int id)
        {
            return await _platformRepository.GetPlatformByIdAsync(id);
        }

        public async Task<List<Platform>> GetAllPlatformsAsync()
        {
            return await _platformRepository.GetAllPlatformsAsync();
        }

        public async Task<Platform?> GetPlatformByNameAsync(string name)
        {
            return await _platformRepository.GetPlatformByNameAsync(name);
        }
    }
}
