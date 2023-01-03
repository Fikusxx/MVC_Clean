using Clean.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            var descriptor = services
                    .FirstOrDefault(x => x.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            // if there's a real db service - remove it
            if (descriptor != null)
                services.Remove(descriptor);

            // create new in memory db every time we run tests
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("DatabaseForTesting");
            });
        });
    }
}
