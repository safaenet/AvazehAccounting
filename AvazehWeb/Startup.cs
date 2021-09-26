using DataLibraryCore.DataAccess.CollectionManagers;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using DataLibraryCore.Models.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<ICollectionManager<ProductModel, IProcessor<ProductModel>>, ICollectionManager<ProductModel, IProcessor<ProductModel>>>()
                .AddScoped<ICollectionManager<CustomerModel, IProcessor<CustomerModel>>, ICollectionManager<CustomerModel, IProcessor<CustomerModel>>>()
                .AddScoped<ICollectionManager<ChequeModel, IProcessor<ChequeModel>>, ICollectionManager<ChequeModel, IProcessor<ChequeModel>>>()
                .AddScoped<IInvoiceCollectionManager, InvoiceCollectionManager>()

                .AddScoped<IProcessor<ProductModel>, IProcessor<ProductModel>>()
                .AddScoped<IProcessor<CustomerModel>, IProcessor<CustomerModel>>()
                .AddScoped<IProcessor<ChequeModel>, IProcessor<ChequeModel>>()
                .AddScoped<IInvoiceProcessor, SqlInvoiceProcessor>()
                .AddSingleton<IDataAccess, SqlDataAccess>();

            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Product}/{action=Index}/{id?}");
            });
        }
    }
}
