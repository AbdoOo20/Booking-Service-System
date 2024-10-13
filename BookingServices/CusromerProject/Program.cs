using BookingServices.Data;
using BookingServices.Services;
using CusromerProject.DTO.Book;
using CusromerProject.DTO.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using CusromerProject.DTO.Categories;
using CusromerProject.DTO.Review;
using CustomerProject.Services;

namespace CusromerProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Database (Entity Framework Core)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<BookingServices.Data.ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Configure Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddHttpClient();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration["JwtSettings:SecretKey"]))
                };
            });

            // Configure Memory Caching
            builder.Services.AddMemoryCache(); // Register IMemoryCache

            // Register Repositories
            builder.Services.AddScoped<CategoryRepository>(); // Register CategoryRepository
            builder.Services.AddScoped<ServiceRepository>(); // Register ServiceRepository
            builder.Services.AddScoped<ReviewRepository>(); // Register ReviewRepository
            builder.Services.AddScoped<BookRepository>(); // Register BookRepository
            builder.Services.AddSingleton<PayPalService>(); // Register PayPalService
            
            // Register Other Services
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Configure Controllers and JSON Serialization Options
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Build the application
            var app = builder.Build();

            // Configure Middleware for Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware Configuration
            app.UseCors("AllowAllOrigins");
            app.UseAuthorization();


            // Map Controllers
            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
