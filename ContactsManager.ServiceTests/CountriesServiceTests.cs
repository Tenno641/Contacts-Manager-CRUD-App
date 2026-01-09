using ContactsManager.Core.DTO.Countries.Request;
using ContactsManager.Core.DTO.Countries.Response;
using ContactsManager.Core.Services.Countries;
using Entities.DataAccess;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace ContactsManager.ServiceTests;

public class CountriesServiceTests : IClassFixture<DbContextFixture>
{
    private readonly CountriesService _service;
    public CountriesServiceTests(DbContextFixture dbContextFixture)
    {
        var options = new DbContextOptionsBuilder<PersonsDbContext>()
            .UseSqlite(dbContextFixture.Connection)
            .Options;

        var context = new PersonsDbContext(options);
        context.Database.EnsureCreated();

        CountriesRepository repository = new CountriesRepository(context);
        _service = new CountriesService(repository);
    }

    #region AddCountryTests
    [Fact]
    public async Task AddCountry_ReturnsNull_IfArgumentIsNull()
    {
        // Arrange
        CountryRequest? request = null;

        // Act
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.AddCountryAsync(request));
    }

    [Fact]
    public async Task AddCountry_ThrowsArgumentNull_IfPropertyIsNull()
    {
        CountryRequest request = new CountryRequest() { Name = null };
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddCountryAsync(request));
    }

    [Fact]
    public async Task AddCountry_ThrowsArgument_IfCountryAlreadyExists()
    {
        CountryRequest request = new CountryRequest() { Name = "USA" };
        CountryRequest request2 = new CountryRequest() { Name = "USA" };
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await _service.AddCountryAsync(request);
            await _service.AddCountryAsync(request2);
        });
    }

    [Fact]
    public async Task AddCountry_AddCountryToRepository()
    {
        // Arrange 
        CountryRequest request = new CountryRequest() { Name = "GHANA" };

        // Act
        CountryResponse response = await _service.AddCountryAsync(request);
        IEnumerable<CountryResponse> actualCountries = await _service.GetAllAsync();

        // Assert
        Assert.Contains(response, actualCountries);
        Assert.True(response.Id != Guid.Empty);
    }
    #endregion

    #region GetAll
    [Fact]
    public async Task GetAll_ReturnsEmptyList_NotAddingAnyData()
    {
        IEnumerable<CountryResponse> actualCountries = await _service.GetAllAsync();
        Assert.Empty(actualCountries);
    }

    [Fact]
    public async Task GetAll_ReturnExistingCountries_AddingCountries()
    {
        List<CountryResponse> resultCountries = [];
        IEnumerable<CountryRequest> countriesRequests =
            [
                new CountryRequest(){Name = "COLOMBIA"},
                new CountryRequest(){Name = "SWEDEN"},
                new CountryRequest(){Name = "GERMANY"}
            ];

        foreach (CountryRequest countryRequest in countriesRequests)
        {
            resultCountries.Add(await _service.AddCountryAsync(countryRequest)); // COLOMBIA BEING ADDED TWICE!
        }

        IEnumerable<CountryResponse> actualCountries = await _service.GetAllAsync();

        foreach (CountryResponse expectedCountry in resultCountries)
        {
            Assert.Contains(expectedCountry, actualCountries);
        }
    }

    #endregion

    #region Get
    [Fact]
    public async Task Get_ReturnNull_IfIdIsNull()
    {
        Guid? guid = null;
        CountryResponse? countryResponse = await _service.GetAsync(guid);
        Assert.Null(countryResponse);
    }

    [Fact]
    public async Task Get_ReturnCountry_IfIdIsValid()
    {
        // Arrange
        CountryRequest request = new CountryRequest() { Name = "LONDON" };
        CountryResponse response = await _service.AddCountryAsync(request);

        // Act
        CountryResponse? countryResponse = await _service.GetAsync(response.Id);

        // Assert
        Assert.Equal(response, countryResponse);
    }
    #endregion
}
