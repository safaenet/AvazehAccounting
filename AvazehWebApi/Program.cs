using Serilog;
using Serilog.Events;
using DataLibraryCore.DataAccess;
using DataLibraryCore.DataAccess.CollectionManagers;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using System.Text;
using Swashbuckle.AspNetCore.Filters;

Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.File(@"log\LogFile.txt")
                    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Standard authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        //ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add services to the container.
builder.Services
                .AddScoped<IUserProcessor, SqlUserProcessor>()
                .AddScoped<IGeneralProcessor<ProductModel>, SqlProductProcessor<ProductModel, ProductValidator>>()
                .AddScoped<IGeneralProcessor<CustomerModel>, SqlCustomerProcessor<CustomerModel, PhoneNumberModel, CustomerValidator>>()
                .AddScoped<IChequeProcessor, SqlChequeProcessor>()
                .AddScoped<IInvoiceProcessor, SqlInvoiceProcessor>()
                .AddScoped<ITransactionProcessor, SqlTransactionProcessor>()
                .AddScoped<IAppSettingsManager, AppSettingsManager>()
                .AddSingleton<IDataAccess, SqlDataAccess>()

                .AddScoped(typeof(IGeneralCollectionManager<ProductModel, IGeneralProcessor<ProductModel>>), typeof(ProductCollectionManager))
                .AddScoped(typeof(IGeneralCollectionManager<CustomerModel, IGeneralProcessor<CustomerModel>>), typeof(CustomerCollectionManager))
                .AddScoped(typeof(IChequeCollectionManager), typeof(ChequeCollectionManager))

                .AddScoped<IInvoiceCollectionManager, InvoiceCollectionManager>()
                .AddScoped<ITransactionCollectionManager, TransactionCollectionManager>()
                .AddScoped<ITransactionItemCollectionManager, TransactionItemCollectionManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AvazehWeb
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder.UseStartup<Startup>();
//                });
//    }
//}