using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class AudioTrackRepository : IAudioTrackRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public AudioTrackRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<AudioTrack> AddAudioTrackAsync(AudioTrack audioTrack)
        {
            _db.AudioTracks.Add(audioTrack);
            await _db.SaveChangesAsync();
            return audioTrack;
        }

        public async Task<AudioTrack> EditAudioTrackAsync(AudioTrack audioTrack)
        {
            _db.AudioTracks.Update(audioTrack);
            await _db.SaveChangesAsync();
            return audioTrack;
        }

        public async Task<bool> DeleteAudioTrackAsync(int audioTrackId)
        {
            var audioTrack = await _db.AudioTracks.FindAsync(audioTrackId);
            if (audioTrack != null)
            {
                _db.AudioTracks.Remove(audioTrack);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<AudioTrack?> GetAudioTrackByIdAsync(int audioTrackId)
        {
            return await _db.AudioTracks.FindAsync(audioTrackId);
        }

        public async Task<List<AudioTrack>> GetAllAudioTracksAsync()
        {
            return await _db.AudioTracks.ToListAsync();
        }

        public async Task<AudioTrack?> GetAudioTracksByLanguageAsync(string language)
        {
            return await _db.AudioTracks.FirstOrDefaultAsync(at => at.Language == language);
        }
    }
}
