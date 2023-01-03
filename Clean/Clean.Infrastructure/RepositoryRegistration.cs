using Clean.Core.Domain.RepositoryContracts;
using Clean.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Infrastructure;

public static class RepositoryRegistration
{
    public static IServiceCollection RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();

        return services;
    }
}
