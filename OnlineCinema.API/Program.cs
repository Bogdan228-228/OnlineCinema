using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OnlineCinemaDbContext>(options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OnlineCinemaDbContext"), 
        sqlServerOptions => sqlServerOptions.CommandTimeout(30)));

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
