using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class AudioTrackService : IAudioTrackService
    {
        private readonly IAudioTrackRepository _audioTrackRepository;

        public AudioTrackService(IAudioTrackRepository audioTrackRepository)
        {
            _audioTrackRepository = audioTrackRepository;
        }

        public async Task<AudioTrack> AddAudioTrackAsync(string language)
        {
            var audioTrack = new AudioTrack
            {
                Language = language
            };
            return await _audioTrackRepository.AddAudioTrackAsync(audioTrack);
        }

        public async Task<AudioTrack?> EditAudioTrackAsync(int audioTrackId, string language)
        {
            var audioTrack = await _audioTrackRepository.GetAudioTrackByIdAsync(audioTrackId);
            if (audioTrack == null)
            {
                return null;
            }
            audioTrack.Language = language;
            return await _audioTrackRepository.EditAudioTrackAsync(audioTrack);
        }

        public async Task<bool> DeleteAudioTrackAsync(int audioTrackId)
        {
            var audioTrack = await _audioTrackRepository.GetAudioTrackByIdAsync(audioTrackId);
            if (audioTrack == null)
            {
                return false;
            }
            return await _audioTrackRepository.DeleteAudioTrackAsync(audioTrack.Id);
        }

        public async Task<AudioTrack?> GetAudioTrackByIdAsync(int audioTrackId)
        {
            return await _audioTrackRepository.GetAudioTrackByIdAsync(audioTrackId);
        }

        public async Task<List<AudioTrack>> GetAllAudioTracksAsync()
        {
            return await _audioTrackRepository.GetAllAudioTracksAsync();
        }

        public async Task<AudioTrack?> GetAudioTrackByLanguageAsync(string language)
        {
            return await _audioTrackRepository.GetAudioTrackByLanguageAsync(language);
        }
    }
}
