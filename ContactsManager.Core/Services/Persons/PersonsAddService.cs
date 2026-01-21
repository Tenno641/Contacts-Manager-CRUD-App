using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO.Persons.Response;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.ServiceContracts.Persons;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Services.Persons;

public class PersonsAddService : IPersonsAddService
{
    private readonly IPersonsRepository _repository;
    public PersonsAddService(IPersonsRepository repository)
    {
        _repository = repository;
    }
    public async Task<PersonResponse> AddPersonAsync(PersonRequest? personRequest)
    {
        ArgumentNullException.ThrowIfNull(personRequest);
        (bool isValid, IReadOnlyCollection<ValidationResult> validationResults) objectValidation = ValidationHelper.ValidateObject(personRequest);

        if (!objectValidation.isValid) throw new ArgumentException(string.Join(",", objectValidation.validationResults.Select(result => result.ErrorMessage)));

        Person person = personRequest.ToPerson();
        person.Id = Guid.NewGuid();

        await _repository.AddPersonAsync(person);
        //InsertPersonStoredProcedure(person);

        return PersonResponseExtension.ToPersonResponse.Compile().Invoke(person);
    }
    public async Task<IEnumerable<PersonResponse>> AddRangeAsync(IEnumerable<PersonRequest> persons)
    {
        IEnumerable<Person> requests = persons.Select(person => person.ToPerson());
        IEnumerable<Person> responses = await _repository.AddRangeAsync(requests);
        return responses.Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person));
    }
}
