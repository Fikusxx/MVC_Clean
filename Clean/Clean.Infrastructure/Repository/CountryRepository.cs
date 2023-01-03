using Clean.Core.Domain.Entities;
using Clean.Core.Domain.RepositoryContracts;
using Clean.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Clean.Infrastructure.Repository;

public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext db;

    public CountryRepository(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<Country> AddAsync(Country country)
    {
        await db.Countries.AddAsync(country);
        await db.SaveChangesAsync();

        return country;
    }

    public async Task<List<Country>> GetAllAsync()
    {
        var countries = await db.Countries.ToListAsync();

        return countries;
    }

    public async Task<Country?> GetByIdAsync(Guid id)
    {
        var country = await db.Countries.FirstOrDefaultAsync(x => x.CountryId == id);

        return country;
    }

    public async Task<Country?> GetByNameAsync(string name)
    {
        var country = await db.Countries.FirstOrDefaultAsync(x => x.Name == name);

        return country;
    }
}
