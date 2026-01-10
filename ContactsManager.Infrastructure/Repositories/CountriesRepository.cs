using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;

public class CountriesRepository : ICountriesRepository
{
    private readonly PersonsDbContext _dbContext;
    public CountriesRepository(PersonsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Country> AddAsync(Country country)
    {
        _dbContext.Countries.Add(country);
        await _dbContext.SaveChangesAsync();
        return country;
    }

    public async Task<IEnumerable<Country>> AddRangeAsync(IEnumerable<Country> countries)
    {
        _dbContext.Countries.AddRange(countries);
        await _dbContext.SaveChangesAsync();
        return countries;
    }

    public async Task<IEnumerable<Country>> AllAsync()
    {
        return await _dbContext.Countries
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Country?> GetAsync(Guid id)
    {
        return await _dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(country => country.Id == id);
    }
}
