using OnlineCinema.Logic.Dto;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IUserStatistic
    {
        Task<UserStatsDto> GetUserStatisticAsync(Guid userId);
        Task InvalidateCacheAsync(Guid userId);
    }
}