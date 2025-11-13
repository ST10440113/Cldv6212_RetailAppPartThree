using Cldv6212_RetailAppPartThree.Data;
using Cldv6212_RetailAppPartThree.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Cldv6212_RetailAppPartThree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<Cldv6212_RetailAppPartThreeContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Cldv6212_RetailAppPartThreeContext") ?? throw new InvalidOperationException("Connection string 'Cldv6212_RetailAppPartThreeContext' not found.")));


            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllersWithViews();



            builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });



            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // GDPR: essential cookie
                options.Cookie.Name = ".mvc_auth_lite";
            });

            //register tableStorage with configuration
            builder.Services.AddSingleton(new TableService(configuration.GetConnectionString("AzureStorage")));

            //register blobStorage with configuration
            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));

            //register queueStorage with configuration
            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connectionString, "orders");
            }

            );

            //register fileStorage with configuration
            builder.Services.AddSingleton<FileService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new FileService(connectionString, "uploads");
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Products}/{action=UserIndex}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
