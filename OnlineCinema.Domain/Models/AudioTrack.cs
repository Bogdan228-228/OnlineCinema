using System.Text.Json.Serialization;

namespace OnlineCinema.Domain.Models
{
    public class AudioTrack
    {
        public int Id { get; set; }
        public string Language { get; set; }
        [JsonIgnore]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
