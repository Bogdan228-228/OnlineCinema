using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IAudioTrackService
    {
        Task<AudioTrack> AddAudioTrackAsync(string language);
        Task<bool> DeleteAudioTrackAsync(int audioTrackId);
        Task<AudioTrack?> EditAudioTrackAsync(int audioTrackId, string language);
        Task<List<AudioTrack>> GetAllAudioTracksAsync();
        Task<AudioTrack?> GetAudioTrackByIdAsync(int audioTrackId);
        Task<AudioTrack?> GetAudioTracksByLanguageIdAsync(string language);
    }
}
