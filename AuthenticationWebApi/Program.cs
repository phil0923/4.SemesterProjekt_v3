using AuthenticationWebApi.Extensions;
using JwtAuthenticationManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthDb(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSingleton<JwtTokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.EnsureAuthDbIsCreated();

app.UseAuthorization();

app.MapControllers();

app.Run();
