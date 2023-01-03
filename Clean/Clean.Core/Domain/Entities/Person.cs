using System.ComponentModel.DataAnnotations;

namespace Clean.Core.Domain.Entities;

public class Person
{
    [Key]
    public Guid PersonId { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool RecieveNewsLetters { get; set; }

    public Country? Country { get; set; }
}
