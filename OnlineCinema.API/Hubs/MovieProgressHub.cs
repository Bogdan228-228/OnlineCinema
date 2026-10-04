using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OnlineCinema.API.Hubs
{
    [Authorize(Roles = "Admin")]
    public class MovieProgressHub : Hub
    {
        public async Task JoinMovieGroup(string movieId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"movie-{movieId}");
        }

        public async Task LeaveMovieGroup(string movieId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"movie-{movieId}");
        }
    }
}