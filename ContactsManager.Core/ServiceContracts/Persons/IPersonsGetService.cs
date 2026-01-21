using ContactsManager.Core.DTO.Persons;
using ContactsManager.Core.DTO.Persons.Response;

namespace ContactsManager.Core.ServiceContracts.Persons;

public interface IPersonsGetService
{
    Task<PersonResponse?> GetAsync(Guid? id);
    Task<IEnumerable<PersonResponse>> GetAllAsync();
    Task<IEnumerable<PersonResponse>> FilterAsync(string searchBy, string? searchString);
    Task<IEnumerable<PersonResponse>> OrderAsync(IEnumerable<PersonResponse> data, string sortBy, SortOrderOptions sortOptions);
    Task<MemoryStream> GetPersonsCsvAsync();
    Task<MemoryStream> GetPersonsExcelAsync();
}
