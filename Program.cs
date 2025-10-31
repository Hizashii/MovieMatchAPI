using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieMatch.Api.Data;
using MovieMatch.Api.Services;
using MovieMatch.Api.Repositories;
using MovieMatch.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Toggle in-memory mode for simple testing (no DB required)
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
if (!useInMemory)
{
    // EF Core with SQLite
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// JWT Auth
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret ?? "dev-secret-change"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddScoped<JwtTokenService>();

// Repositories
if (useInMemory)
{
    builder.Services.AddSingleton<IMovieRepository, InMemoryMovieRepository>();
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
}
else
{
    // For now, reuse in-memory repositories even in DB mode (could be replaced with EF-backed repos)
    builder.Services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
    builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
}

// Filters
builder.Services.AddSingleton<RequireAuthUnlessInMemory>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieMatch API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();

if (!useInMemory)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redirect root to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

namespace MovieMatch.Api.Services
{
    public sealed class JwtOptions
    {
        public string? Secret { get; set; }
        public string Issuer { get; set; } = "MovieMatch";
        public string Audience { get; set; } = "MovieMatchAudience";
        public int ExpirationMinutes { get; set; } = 120;
    }
}

