
using Clean.Core.Domain.Entities;

namespace Clean.Core.Domain.RepositoryContracts;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllAsync();
    public Task<Country?> GetByIdAsync(Guid id);
    public Task<Country?> GetByNameAsync(string name);
    public Task<Country> AddAsync(Country country);
}
