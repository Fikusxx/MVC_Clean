using Clean.Core.Domain.Entities;
using Clean.Core.Domain.RepositoryContracts;
using Clean.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Clean.Infrastructure.Repository;

public class PersonRepository : IPersonRepository
{
    private readonly ApplicationDbContext db;

    public PersonRepository(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<Person> AddAsync(Person person)
    {
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        return person;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var person = await db.Persons.FirstOrDefaultAsync(x => x.PersonId == id);

        if (person == null)
            return false;

        db.Persons.Remove(person);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<List<Person>> GetAllAsync()
    {
        var persons = await db.Persons.Include(x => x.Country).ToListAsync();

        return persons;
    }

    public async Task<Person?> GetByIdAsync(Guid id)
    {
        var person = await db.Persons.Include(x => x.Country).FirstOrDefaultAsync(x => x.PersonId == id);

        return person;
    }

    public async Task<List<Person>> GetFilteredPersonsAsync(Expression<Func<Person, bool>> predicate)
    {
        var persons = await db.Persons.Include(x => x.Country).Where(predicate).ToListAsync();

        return persons;
    }

    public async Task<Person> UpdateAsync(Person person)
    {
        db.Update(person);
        await db.SaveChangesAsync();

        return person;
    }
}
