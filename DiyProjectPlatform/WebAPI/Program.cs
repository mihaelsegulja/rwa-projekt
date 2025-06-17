using Core.Context;
using Core.Interfaces;
using Core.Mappings;
using Core.Middleware;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Config;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1",
        new OpenApiInfo { Title = "DIY Project Platform Web API", Version = "v1" });

    option.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter valid JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
                new List<string>()
            }
        });
});

builder.Services.AddDbContext<DbDiyProjectPlatformContext>(options => {
    options.UseSqlServer("name=ConnectionStrings:DefaultConnection");
});

JwtTokenConfig.TokenSecret = builder.Configuration["JWT:SecureKey"];
JwtTokenConfig.TokenIssuer = builder.Configuration["JWT:Issuer"];
JwtTokenConfig.TokenAudience = builder.Configuration["JWT:Audience"];
JwtTokenConfig.TokenExpiration = int.Parse(builder.Configuration["JWT:ExpiryInMinutes"]);

// Configure JWT security services
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenConfig.TokenSecret))
        };
    });

builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddAutoMapper(
    typeof(CoreMappingProfile)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
