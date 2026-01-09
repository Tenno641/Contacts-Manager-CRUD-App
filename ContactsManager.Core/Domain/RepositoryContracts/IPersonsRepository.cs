using ContactsManager.Core.Domain.Entities;
using System.Linq.Expressions;

namespace ContactsManager.Core.Domain.RepositoryContracts;

public interface IPersonsRepository
{
    Task<Person> AddPersonAsync(Person person);
    Task<IEnumerable<Person>> AddRangeAsync(IEnumerable<Person> persons);
    Task<Person?> GetAsync(Guid id);
    Task<IEnumerable<Person>> AllAsync();
    Task<Person> UpdateAsync(Person person);
    Task<IEnumerable<Person>> FilterAsync(Expression<Func<Person, bool>> predicate);
    Task<bool> RemoveAsync(Guid id);
}
