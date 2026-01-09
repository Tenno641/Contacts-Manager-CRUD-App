using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO.Persons;
using ContactsManager.Core.DTO.Persons.Response;
using ContactsManager.Core.ServiceContracts.Persons;
using CsvHelper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace ContactsManager.Core.Services.Persons;
public class PersonsGetService : IPersonsGetService
{
    private readonly IPersonsRepository _repository;
    public PersonsGetService(IPersonsRepository repository)
    {
        _repository = repository;
    }
    public async Task<PersonResponse?> GetAsync(Guid? id)
    {
        if (id is null) return null;

        Person? person = await _repository.GetAsync(id.Value);
        if (person is null) return null;

        return PersonResponseExtension.ToPersonResponse.Compile().Invoke(person);
    }
    public async Task<IEnumerable<PersonResponse>> GetAllAsync()
    {
        return (await _repository.AllAsync()).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList();
    }
    public async Task<IEnumerable<PersonResponse>> FilterAsync(string searchBy, string? searchString)
    {
        int age = 0;
        if (searchString is null) return await GetAllAsync();
        if (searchBy.Equals(nameof(PersonResponse.Age)) && !int.TryParse(searchString, out age))
            return await GetAllAsync(); 

        return searchBy switch
        {
            nameof(PersonResponse.Name) => (await _repository.FilterAsync(person => person.Name == null || person.Name.Contains(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.Email) => (await _repository.FilterAsync(person => person.Email == null || person.Email.Contains(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.DateOfBirth) => (await _repository.FilterAsync(person => person.DateOfBirth == null || person.DateOfBirth.Value.ToString("yyyy-mm-ddd").Contains(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.Gender) => (await _repository.FilterAsync(person => person.Gender == null || person.Gender.Equals(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.CountryName) => (await _repository.FilterAsync(person => person.Country == null || person.Country.Name == null || person.Country.Name.Contains(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.Address) => (await _repository.FilterAsync(person => person.Address == null || person.Address.Contains(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.ReceiveNewsLetter) => (await _repository.FilterAsync(person => person.ReceiveNewsLetter.Equals(searchString))).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).ToList(),

            nameof(PersonResponse.Age) => (await _repository.AllAsync()).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)).Where(person => person.Age == age).ToList(),
            _ => await GetAllAsync()
        };
    }
    private async Task<IEnumerable<PersonResponse>> OrderGenericAsync<T>(IEnumerable<PersonResponse> data, Func<PersonResponse, T> selector, SortOrderOptions sortOrderOptions)
    {
        return sortOrderOptions switch
        {
            SortOrderOptions.Descending => data.OrderByDescending(selector).ToList(),
            SortOrderOptions.Ascending => data.OrderBy(selector).ToList(),
            _ => await GetAllAsync()
        };
    }
    public async Task<IEnumerable<PersonResponse>> OrderAsync(IEnumerable<PersonResponse> data, string sortBy, SortOrderOptions sortOptions)
    {
        return sortBy switch
        {
            "Name" => await OrderGenericAsync(data, person => person.Name, sortOptions),
            "Email" => await OrderGenericAsync(data, person => person.Email, sortOptions),
            "DateOfBirth" => await OrderGenericAsync(data, person => person.DateOfBirth, sortOptions),
            "Age" => await OrderGenericAsync(data, person => person.Age, sortOptions),
            "Gender" => await OrderGenericAsync(data, person => person.Gender, sortOptions),
            "Country" => await OrderGenericAsync(data, person => person.CountryName, sortOptions),
            "Address" => await OrderGenericAsync(data, person => person.Address, sortOptions),
            "ReceiveNewsLetters" => await OrderGenericAsync(data, person => person.ReceiveNewsLetter, sortOptions),
            _ => await GetAllAsync()
        };
    }

    public async Task<MemoryStream> GetPersonsCsvAsync()
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);
        CsvWriter csvWriter = new CsvWriter(streamWriter, culture: CultureInfo.InvariantCulture, leaveOpen: true);
        csvWriter.WriteHeader<PersonResponse>();

        await csvWriter.NextRecordAsync();

        IEnumerable<PersonResponse> persons = await GetAllAsync();
        await csvWriter.WriteRecordsAsync<PersonResponse>(persons);
        await csvWriter.FlushAsync();
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcelAsync()
    {
        MemoryStream memoryStream = new();
        ExcelPackage.License.SetNonCommercialPersonal("Tenno641");
        using ExcelPackage excelPackage = new ExcelPackage(memoryStream);

        ExcelWorksheet sheet = excelPackage.Workbook.Worksheets.Add("Persons");

        sheet.Cells["A1"].Value = "ID";
        sheet.Cells["B1"].Value = "Name";
        sheet.Cells["C1"].Value = "Email";
        sheet.Cells["D1"].Value = "Date Of Birth";

        using ExcelRange sheetHeaders = sheet.Cells["A1:D1"];
        sheetHeaders.Style.Fill.SetBackground(Color.AntiqueWhite);
        sheetHeaders.Style.Font.Bold = true;
        sheetHeaders.Style.Fill.PatternType = ExcelFillStyle.DarkVertical;
        int row = 2;
        IEnumerable<PersonResponse> persons = await GetAllAsync();

        foreach (PersonResponse person in persons)
        {
            sheet.Cells[row, 1].Value = person.Id;
            sheet.Cells[row, 2].Value = person.Name;
            sheet.Cells[row, 3].Value = person.Email;
            sheet.Cells[row, 4].Value = person.DateOfBirth?.ToString("yyyy-mm-dd");
            row++;
        }
        sheet.Cells["A1:D5"].AutoFitColumns();
        await excelPackage.SaveAsync();
        memoryStream.Position = 0;
        return memoryStream;
    }
}
