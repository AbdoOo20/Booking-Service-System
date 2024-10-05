using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using CusromerProject.DTO.Categories;
using CusromerProject.DTO.Services;
using Microsoft.AspNetCore.Identity; // Import your DTO namespace

namespace CusromerProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Configure Entity Framework Core
            builder.Services.AddDbContext<BookingServices.Data.ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<BookingServices.Data.ApplicationDbContext>();

            // Add memory caching
            builder.Services.AddMemoryCache(); // Register IMemoryCache

            // Register your repositories
            builder.Services.AddScoped<CategoryRepository>(); // Register CategoryRepository

            builder.Services.AddScoped<ServiceRepository>(); // Register ServiceRepository


            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure JSON serialization options
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseCors("AllowAllOrigins");

            app.MapControllers();

            app.Run();
        }
    }
}
