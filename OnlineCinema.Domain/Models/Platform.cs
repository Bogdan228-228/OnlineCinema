namespace OnlineCinema.Domain.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
