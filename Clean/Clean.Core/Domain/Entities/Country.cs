using System.ComponentModel.DataAnnotations;

namespace Clean.Core.Domain.Entities;

public class Country
{
    [Key]
    public Guid CountryId { get; set; }
    public string Name { get; set; }
    public ICollection<Person>? Persons { get; set; }
}