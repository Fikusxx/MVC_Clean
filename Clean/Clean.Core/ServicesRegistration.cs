using Clean.Core.ServiceContracts;
using Clean.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Core;

public static class ServicesRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IPersonGetterService, PersonGetterService>();
        services.AddScoped<IPersonAdderService, PersonAdderService>();
        services.AddScoped<IPersonUpdaterService, PersonUpdaterService>();
        services.AddScoped<IPersonDeleterService, PersonDeleterService>();
        services.AddScoped<IPersonSorterService, PersonSorterService>();

        return services;
    }
}
