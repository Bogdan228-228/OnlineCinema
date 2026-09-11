namespace OnlineCinema.Domain.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImgUrl { get; set; } = "";
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public decimal Review { get; set; }
        public int RecommendedAge { get; set; }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public DateOnly DateRealise { get; set; }
        public TimeSpan Duration { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
        public string Description { get; set; }
        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
        public string Country { get; set; }
        public ICollection<AudioTrack> AudioTracks { get; set; } = new List<AudioTrack>();
        public ICollection<Platform> Platforms { get; set; } = new List<Platform>();
        public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
