using Clean.Core.Domain.Entities;
using Clean.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clean.Core.DTO.PersonDTO;


public class PersonAddRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public GenderOptions? Gender { get; set; }
    [Required]
    public Guid? CountryId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool RecieveNewsLetters { get; set; }

    public Person ToPerson()
    {
        return new Person()
        {
            Name = Name,
            Email = Email,
            Address = Address,
            RecieveNewsLetters = RecieveNewsLetters,
            CountryId = CountryId,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString()
        };
    }
}
