using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.ServiceContracts.Countries;
using ContactsManager.Core.ServiceContracts.Persons;
using ContactsManager.Core.Services.Countries;
using ContactsManager.Core.Services.Persons;
using ContactsManager.Web.Filters.ActionFilters;
using Entities.DataAccess;
using Microsoft.EntityFrameworkCore;
using Repository;
using Rotativaio.AspNetCore;

namespace ContactsManager.Web.Startup;

public static class ServicesConfigurationExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllersWithViews(options =>
        {
            ILogger<CustomActionFilters> logger = services.BuildServiceProvider().GetRequiredService<ILogger<CustomActionFilters>>();

            options.Filters.Add(new CustomActionFilters(logger));
        });

        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IPersonsRepository, PersonsRepository>();

        services.AddScoped<ICountriesService, CountriesService>();
        services.AddScoped<IPersonsAddService, PersonsAddService>();
        services.AddScoped<IPersonsUpdateService, PersonsUpdateService>();
        services.AddScoped<IPersonsDeleteService, PersonsDeleteService>();
        services.AddScoped<IPersonsGetService, PersonsGetService>();

        services.AddDbContext<PersonsDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });

        services.AddHttpLogging(options =>
        {
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.None;
        });

        if (!environment.IsEnvironment("IntegrationTesting"))
        {
            services.AddRotativaIo("https://api.rotativa.io", configuration["rotativaApiKey"] ?? throw new InvalidOperationException("RotativaApiKey is missing"));
        }
    }
}
