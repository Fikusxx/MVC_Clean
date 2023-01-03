using Clean.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Clean.Core.Domain.RepositoryContracts;

public interface IPersonRepository
{
    public Task<Person?> GetByIdAsync(Guid id);
    public Task<List<Person>> GetAllAsync();
    public Task<List<Person>> GetFilteredPersonsAsync(Expression<Func<Person, bool>> predicate);
    public Task<Person> AddAsync(Person person);
    public Task<Person> UpdateAsync(Person person);
    public Task<bool> DeleteAsync(Guid id);
}
