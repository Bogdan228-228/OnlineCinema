namespace OnlineCinema.Domain.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
