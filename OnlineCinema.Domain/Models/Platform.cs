namespace OnlineCinema.Domain.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
