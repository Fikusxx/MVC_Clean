using Clean.Core.Domain.Entities;
using Clean.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Clean.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    { }

    public virtual DbSet<Person> Persons { get; set; } = null!;
    public virtual DbSet<Country> Countries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SeedData(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Countries
        string countriesJson = File.ReadAllText("countries.json");
        List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

        if (countries == null)
            return;

        foreach (Country country in countries)
            modelBuilder.Entity<Country>().HasData(country);

        // Persons
        string personsJson = File.ReadAllText("persons.json");
        List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

        if (persons == null)
            return;

        foreach (Person person in persons)
            modelBuilder.Entity<Person>().HasData(person);
    }
}
