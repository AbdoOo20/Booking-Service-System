using BookingServices.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using BookingServices.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookingServices.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookingServices.Hubs;

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
                options.UseSqlServer(connectionString), ServiceLifetime.Scoped);



            // Add Quartz services
            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory(); // Use DI for Jobs

                // Define the job and tie it to the UpdateServiceProviderBalancesJob class
                var jobKey = new JobKey("UpdateServiceProviderBalancesJob");
                q.AddJob<UpdateServiceProviderBalancesJob>(opts => opts.WithIdentity(jobKey));

                // Create a trigger that fires at 12 AM every day
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)  // Link the trigger to the job
                    .WithIdentity("UpdateServiceProviderBalancesTrigger")
                    //.WithCronSchedule("0/10 * * * * ?") // Cron expression for daily each 10 second
                    .WithCronSchedule("0 0 0 * * ?") // Cron expression for daily at 12 AM
                    .WithDescription("Daily task to update service provider balances at 12 AM"));
            });
            // Add Quartz.NET hosted service
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(); // This will allow console logging


            // SignalR Service
            builder.Services.AddSignalR();

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

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
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

            // SignalR Middleware
            //app.MapHub<Hubs.AdminNotification>("/AdminNotification");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AdminNotification>("/AdminNotification");
            });

            app.Run();
        }
    }
}
