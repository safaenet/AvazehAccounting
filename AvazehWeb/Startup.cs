using DataLibraryCore.DataAccess.CollectionManagers;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using System;

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
            //services.AddControllers(options => options.EnableEndpointRouting = false);
            services
                .AddScoped<IGeneralProcessor<ProductModel>, SqlProductProcessor<ProductModel, ProductValidator>>()
                .AddScoped<IGeneralProcessor<CustomerModel>, SqlCustomerProcessor<CustomerModel, PhoneNumberModel, CustomerValidator>>()
                .AddScoped<IGeneralProcessor<ChequeModel>, SqlChequeProcessor<ChequeModel, ChequeEventModel, ChequeValidator>>()
                .AddScoped<IInvoiceProcessor, SqlInvoiceProcessor>()
                .AddScoped<ITransactionProcessor, SqlTransactionProcessor>()
                .AddSingleton<IDataAccess, SqlDataAccess>()

                .AddScoped(typeof(IGeneralCollectionManager<ProductModel, IGeneralProcessor<ProductModel>>), typeof(ProductCollectionManager))
                .AddScoped(typeof(IGeneralCollectionManager<CustomerModel, IGeneralProcessor<CustomerModel>>), typeof(CustomerCollectionManager))
                .AddScoped(typeof(IGeneralCollectionManager<ChequeModel, IGeneralProcessor<ChequeModel>>), typeof(ChequeCollectionManager))

                .AddScoped<IInvoiceCollectionManager, InvoiceCollectionManager>()
                .AddScoped<ITransactionCollectionManager, TransactionCollectionManager>()
                .AddScoped<ITransactionItemCollectionManager, TransactionItemCollectionManager>();

            services.AddControllersWithViews(options => options.EnableEndpointRouting = false);
            //services.AddDistributedMemoryCache();

            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(60);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});
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
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseSession();
            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Product}/{action=Index}/{id?}");
            });
        }
    }
}