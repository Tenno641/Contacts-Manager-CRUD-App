using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.ServiceContracts.Persons;

namespace ContactsManager.Core.Services.Persons;
public class PersonsDeleteService : IPersonsDeleteService
{
    private readonly IPersonsRepository _repository;
    public PersonsDeleteService(IPersonsRepository repository)
    {
        _repository = repository;
    }
    public async Task<bool> DeleteAsync(Guid? id)
    {
        ArgumentNullException.ThrowIfNull(id);

        Person? person = await _repository.GetAsync(id.Value);
        if (person is null) return false;

        return await _repository.RemoveAsync(id.Value);
    }
}
