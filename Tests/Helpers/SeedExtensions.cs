using OnlineCinema.Domain.Models;
using OnlineCinema.Tests.Fixtures;

namespace OnlineCinema.Tests.Helpers;

public static class SeedExtensions
{
    public static async Task<Movie> SeedMovieAsync(
        this CinemaWebApplicationFactory factory, string title = "Seeded movie", string country = "UA")
    {
        Movie? movie = null;
        await factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            movie = TestData.NewMovie(category.Id, title);
            movie.Country = country;
            db.Movies.Add(movie);
            await db.SaveChangesAsync();
        });
        return movie!;
    }
}