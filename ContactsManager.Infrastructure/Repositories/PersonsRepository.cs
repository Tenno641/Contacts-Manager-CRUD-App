using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using Entities.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository;

public class PersonsRepository : IPersonsRepository
{
    private readonly PersonsDbContext _dbContext;
    public PersonsRepository(PersonsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Person> AddPersonAsync(Person person)
    {
        _dbContext.Add(person);
        await _dbContext.SaveChangesAsync();
        return person;
    }

    public async Task<IEnumerable<Person>> AllAsync()
    {
        return await _dbContext.Persons
            .AsNoTracking()
            .Include(person => person.Country)
            .ToListAsync();
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        Person? person = await GetAsync(id);
        if (person is null) return false;

        _dbContext.Remove(person);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Person>> FilterAsync(Expression<Func<Person, bool>> predicate)
    {
        return await _dbContext.Persons
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Person?> GetAsync(Guid id)
    {
        return await _dbContext.Persons
            .Include(person => person.Country)
            .FirstOrDefaultAsync(person => person.Id == id);
    }

    public async Task<Person> UpdateAsync(Person person)
    {
        Person? existingPerson = await GetAsync(person.Id);
        if (existingPerson is null)
        {
            return person;
        }

        existingPerson.Name = person.Name;
        existingPerson.Email = person.Email;
        existingPerson.DateOfBirth = person.DateOfBirth;
        existingPerson.CountryId = person.CountryId;
        existingPerson.Gender = person.Gender;
        existingPerson.Address = person.Address;
        existingPerson.ReceiveNewsLetter = person.ReceiveNewsLetter;

        await _dbContext.SaveChangesAsync();
        return existingPerson;
    }

    public async Task<IEnumerable<Person>> AddRangeAsync(IEnumerable<Person> persons)
    {
        _dbContext.Persons.AddRange(persons);
        await _dbContext.SaveChangesAsync();
        return persons;
    }
}
