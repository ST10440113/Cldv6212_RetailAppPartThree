using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cldv6212_RetailAppPartThree.Data;
namespace Cldv6212_RetailAppPartThree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<Cldv6212_RetailAppPartThreeContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Cldv6212_RetailAppPartThreeContext") ?? throw new InvalidOperationException("Connection string 'Cldv6212_RetailAppPartThreeContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
