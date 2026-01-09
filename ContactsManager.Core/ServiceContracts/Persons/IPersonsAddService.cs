using ContactsManager.Core.DTO.Persons.Response;

namespace ContactsManager.Core.ServiceContracts.Persons;
public interface IPersonsAddService
{
    Task<PersonResponse> AddPersonAsync(PersonRequest? personRequest);
    Task<IEnumerable<PersonResponse>> AddRangeAsync(IEnumerable<PersonRequest> persons);
}
