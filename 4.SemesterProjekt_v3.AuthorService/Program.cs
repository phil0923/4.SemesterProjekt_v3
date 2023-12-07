using _4.SemesterProjekt_v3.AuthorService.Extensions;
using _4.SemesterProjekt_v3.AuthorService.PubSub;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorDb(builder.Configuration);
builder.Services.AddScoped<AuthorPublisher>();
builder.Services.AddSingleton<IntegrationEventSenderService>();
builder.Services.AddHostedService<IntegrationEventSenderService>(provider => provider.GetService<IntegrationEventSenderService>());
builder.Services.AddCustomJwtAuthentication();


builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.EnsureAuthorDbIsCreated();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
