namespace OnlineCinema.Domain.Models
{
    public class Actor
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Biography { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
