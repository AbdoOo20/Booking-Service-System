using BookingServices.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using BookingServices.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookingServices.ViewModel;


namespace BookingServices
{
    public class Program
    {
        public static void Main(string[] args) // Abdo
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2097152; // 2 MB limit
            });
         
            builder.Services.AddHttpClient();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });                
                options.AddPolicy("Provider", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy("Customer", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                if (path == "/" || path == "/Home/Index")
                {
                    if (context.User.Identity.IsAuthenticated)
                    {
                        if (context.User.IsInRole("Admin"))
                        {
                            context.Response.Redirect("/AdminHome");
                            return;
                        }
                        else if (context.User.IsInRole("Provider"))
                        {
                            context.Response.Redirect("/ProviderHome");
                            return;
                        }
                    }
                    else 
                    {
                        context.Response.Redirect("/Identity/Account/Login");
                    }
                }
                await next();
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
