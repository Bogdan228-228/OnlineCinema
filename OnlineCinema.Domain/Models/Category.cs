using System.Text.Json.Serialization;

namespace OnlineCinema.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
