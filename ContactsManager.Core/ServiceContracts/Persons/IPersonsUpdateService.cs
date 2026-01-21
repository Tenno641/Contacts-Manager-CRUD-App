using ContactsManager.Core.DTO.Persons.Request;
using ContactsManager.Core.DTO.Persons.Response;

namespace ContactsManager.Core.ServiceContracts.Persons;

public interface IPersonsUpdateService
{
    Task<PersonResponse> UpdateAsync(PersonUpdateRequest? personUpdateRequest);
}
