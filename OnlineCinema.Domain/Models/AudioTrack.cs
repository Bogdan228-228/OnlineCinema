namespace OnlineCinema.Domain.Models
{
    public class AudioTrack
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
