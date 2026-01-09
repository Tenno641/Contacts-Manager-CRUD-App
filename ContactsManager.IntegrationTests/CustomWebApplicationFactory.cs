using Entities.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(service => service.ServiceType == typeof(IDbContextOptionsConfiguration<PersonsDbContext>));

            if (serviceDescriptor is not null) services.Remove(serviceDescriptor);

            services.AddDbContext<PersonsDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestingDatabase");
            });

        });
    }
}
