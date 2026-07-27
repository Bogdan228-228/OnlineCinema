namespace OnlineCinema.Domain.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImgUrl { get; set; } = "";
        public Category Category { get; set; }
        public decimal Review { get; set; }
        public int RecommendedAge { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public DateOnly DateRealise { get; set; }
        public TimeSpan Duration { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
        public string Description { get; set; }
        public ICollection<Actor> Actors { get; set; }
        public string Country { get; set; }
        public ICollection<AudioTrack> AudioTracks { get; set; }
        public ICollection<Platform> Platforms { get; set; }
    }
}
