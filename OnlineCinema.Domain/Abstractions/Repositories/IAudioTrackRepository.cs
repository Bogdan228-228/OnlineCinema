using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IAudioTrackRepository
    {
        Task<AudioTrack> AddAudioTrackAsync(AudioTrack audioTrack);
        Task<AudioTrack> EditAudioTrackAsync(AudioTrack audioTrack);
        Task<AudioTrack> DeleteAudioTrackAsync(int audioTrackId);
        Task<AudioTrack?> GetAudioTrackByIdAsync(int audioTrackId);
        Task<List<AudioTrack>> GetAllAudioTracksAsync();
        Task<AudioTrack?> GetAudioTracksByLanguageIdAsync(string language);
    }
}
