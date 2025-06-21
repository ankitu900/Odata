using Business.Repositories;
using Business.Services;
using Business;
using Odata.Entities;

namespace ClientProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           // Replace with your actual Product entity namespace

            // ... inside Main method, after AddControllersWithViews():

            // Register ODataContainer as IUnitOfWork (scoped)
            builder.Services.AddScoped<IUnitOfWork, ODataContainer>();

            // Register ProductService as IProductService (scoped)
            builder.Services.AddScoped<IProductService, ProductService>();

            // If you want to inject IRepository<Product> directly, you can add:
            builder.Services.AddScoped<IRepository<Product>>(provider =>
            {
                var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
                return unitOfWork.Repository<Product>();
            });



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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
