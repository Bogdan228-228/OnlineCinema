using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateMovieRequest(
        [Required, MaxLength(100)]
        string Title,
        [Required] 
        int CategoryId,
        [Required, Range(0, 10)] 
        decimal Review,
        [Required, Range(0, 18)] 
        int RecommendedAge,
        [Required] 
        DateOnly DateRealise,
        [Required] 
        TimeSpan Duration,
        [Required, MaxLength(1000)]
        string Description,
        [Required, MaxLength(100)] 
        string Country,
        [Required, MaxLength(200)] 
        string ImgUrl,
        List<int>? GenreIds,
        List<Guid>? ActorIds,
        List<int>? AudioTrackIds,
        List<int>? PlatformIds
    );

    public record EditMovieRequest(
        [Required] 
        Guid Id,
        [ MaxLength(100)] 
        string? Title,
        int? CategoryId,
        [Range(0, 10)] 
        decimal? Review,
        [Range(0, 18)] 
        int? RecommendedAge,
        DateOnly? DateRealise,
        TimeSpan? Duration,
        [Range(0, int.MaxValue)]
        int? Likes,
        [Range(0, int.MaxValue)]
        int? Dislikes,
        [MaxLength(1000)]
        string? Description,
        [MaxLength(100)] 
        string? Country,
        [MaxLength(200)] 
        string? ImgUrl,
        List<int>? GenreIds,
        List<Guid>? ActorIds,
        List<int>? AudioTrackIds,
        List<int>? PlatformIds
    );

    public record MovieResponse(
        Guid Id,
        string Title,
        decimal Review,
        int RecommendedAge,
        DateOnly DateRealise,
        TimeSpan Duration,
        string Description,
        string Country,
        string ImgUrl,
        int Likes,
        int Dislikes,
        CategoryResponse Category,
        List<GenreResponse> Genres,
        List<ActorResponse> Actors,
        List<AudioTrackResponse> AudioTracks,
        List<PlatformResponse> Platforms
    );
}
