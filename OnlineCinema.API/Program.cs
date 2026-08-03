using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.DataAccess.Repositories;
using OnlineCinema.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OnlineCinemaDbContext>(options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OnlineCinemaDbContext"), 
        sqlServerOptions => sqlServerOptions.CommandTimeout(30)));

builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<IAudioTrackRepository, AudioTrackRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<IAudioTrackService, AudioTrackService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
