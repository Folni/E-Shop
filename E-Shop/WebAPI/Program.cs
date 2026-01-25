using ETrgovina.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
// 1. Dodaj namespace gdje se nalazi tvoj MappingProfile
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// 2. REGISTRACIJA AUTOMAPPERA
// Ova linija skenira tvoj projekt i pronalazi klasu koja naslje?uje 'Profile'
builder.Services.AddAutoMapper(typeof(MappingProfile));

var secureKey = builder.Configuration["JWT:SecureKey"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey ?? "TvojJakoDugacakISiguranKljuc123!"))
        };
    });

builder.Services.AddDbContext<EtrgovinaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. KONFIGURACIJA KONTROLERA
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // Budu?i da sada koristiš DTO-ove, ReferenceHandler.IgnoreCycles ti tehni?ki 
    // više nije strogo potreban (jer DTO-ovi nemaju kružne veze), 
    // ali je dobro ostaviti ga kao zaštitu.
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Unesite JWT token (npr. 'ey...')",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();