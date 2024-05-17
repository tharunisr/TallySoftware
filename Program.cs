using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer;
using TallySoftware.Services;

namespace TallySoftware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer("name=Connection");
            });
           builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<IStaffService, StaffService>();
                 builder.Services.AddTransient<ICustomerService, CustomerService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=authentication}/{action=login}/{id?}");

            app.Run();
        }
    }
}