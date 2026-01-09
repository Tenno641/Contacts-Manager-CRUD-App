using ContactsManager.Core.DTO.Countries.Request;
using ContactsManager.Core.DTO.Countries.Response;
using Microsoft.AspNetCore.Http;

namespace ContactsManager.Core.ServiceContracts.Countries;

public interface ICountriesService
{
    Task<CountryResponse> AddCountryAsync(CountryRequest countryRequest);
    Task<IEnumerable<CountryResponse>> GetAllAsync();
    Task<CountryResponse?> GetAsync(Guid? id);
    Task<int> ImportFromExcelFile(IFormFile file);
}
