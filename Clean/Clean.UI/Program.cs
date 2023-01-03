using Clean.Core;
using Clean.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Stonks.Filters.ActionFilters;


var builder = WebApplication.CreateBuilder();
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services,
    LoggerConfiguration config) =>
    {
        config.ReadFrom.Configuration(context.Configuration) // reads appSettings
              .ReadFrom.Services(services); // reads services and make them available to Serilog
    });

services.RegisterDatabase(configuration);
services.RegisterRepository();
services.RegisterServices(configuration);

services.AddTransient<ResponseHeaderActionFilter>();
services.AddMvc();



builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties |
    HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseDeveloperExceptionPage();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();




app.Run();

public partial class Program
{

}