// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using _Net.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers (API)
builder.Services.AddControllers();

// Repos
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRepositoryPropietario, PropietariosRepository>();
builder.Services.AddScoped<IRepositoryPagos, PagosRepository>();
builder.Services.AddScoped<IRepositoryInmuebles, InmueblesRepository>();
builder.Services.AddScoped<IRepositoryContratos, ContratosRepository>();
builder.Services.AddScoped<IRepositoryInquilinos, InquilinosRepository>();

// CORS
builder.Services.AddCors(o => o.AddPolicy("mobile", p => p
    .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Swagger (+ esquema Bearer para probar tokens)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inmobiliaria API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",   // en Swagger poné: Bearer <token>
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();         // <= importante para /fotos/*
app.UseCors("mobile");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
