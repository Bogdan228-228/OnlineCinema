namespace OnlineCinema.Domain.Models
{
    public class AudioTrack
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
