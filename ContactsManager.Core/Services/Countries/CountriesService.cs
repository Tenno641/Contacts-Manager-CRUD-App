using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO.Countries.Request;
using ContactsManager.Core.DTO.Countries.Response;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.ServiceContracts.Countries;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ContactsManager.Core.Services.Countries;

public class CountriesService : ICountriesService
{
    private readonly ICountriesRepository _repository;
    public CountriesService(ICountriesRepository repository)
    {
        _repository = repository;
    }
    public async Task<CountryResponse> AddCountryAsync(CountryRequest? countryRequest)
    {
        ArgumentNullException.ThrowIfNull(countryRequest);

        (bool isValid, IReadOnlyCollection<ValidationResult> validationResults) objectValidation = ValidationHelper.ValidateObject(countryRequest);

        if (!objectValidation.isValid)
        {
            string errors = string.Join(",", objectValidation.validationResults.Select(result => result.ErrorMessage));
            throw new ArgumentException(errors);
        }

        Country country = countryRequest.ToCountry();
        country.Id = Guid.NewGuid();

        if (await isDuplicatedAsync(country)) throw new ArgumentException("Country Already Exist.");

        await _repository.AddAsync(country);

        return country.ToCountryResponse();
    }
    public async Task<IEnumerable<CountryResponse>> GetAllAsync()
    {
        return (await _repository.AllAsync()).Select(country => country.ToCountryResponse()).ToList();
    }
    public async Task<CountryResponse?> GetAsync(Guid? id)
    {
        if (id is null) return null;

        Country? country = await _repository.GetAsync(id.Value);
        if (country is null) return null;

        return country.ToCountryResponse();
    }
    private static readonly Expression<Func<Country, CountryResponse>> ToCountryResponseExpression =
    country => new CountryResponse { Id = country.Id, Name = country.Name };
    private async Task<bool> isDuplicatedAsync(Country country)
    {
        return await _repository.GetAsync(country.Id) != null;
    }

    public async Task<int> ImportFromExcelFile(IFormFile file)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Tenno641");
        List<Country> newCountries = new List<Country>();

        MemoryStream stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using ExcelPackage excelPackage = new ExcelPackage(stream);

        ExcelWorksheet sheet = excelPackage.Workbook.Worksheets["Countries"];

        int rowsCount = sheet.Cells.Rows;
        int affectedRows = 0;
        for (int row = 2; row <= rowsCount; row++)
        {
            Country country = new Country() { Name = Convert.ToString(sheet.GetValue(row, 1)) };

            if (string.IsNullOrWhiteSpace(country.Name)) continue;
            if (await isDuplicatedAsync(country)) continue;

            newCountries.Add(country);
            affectedRows++;
        }
        await _repository.AddRangeAsync(newCountries);
        return affectedRows;
    }
}
