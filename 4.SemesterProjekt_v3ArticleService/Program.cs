using _4.SemesterProjekt_v3ArticleService.Extensions;
using _4.SemesterProjekt_v3ArticleService.PubSub;
using JwtAuthenticationManager;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddArticleDb(builder.Configuration);
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "ArticleService", Version = "v1" });
});

var app = builder.Build();

AuthorSubscriber.ListenForIntegrationEvents();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.EnsureArticleDbIsCreated();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
