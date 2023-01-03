
using Clean.Core.Enums;

namespace Clean.Core.DTO.PersonDTO;


public class PersonResponse
{
    public Guid PersonId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string? Address { get; set; }
    public bool RecieveNewsLetters { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is PersonResponse response)
            return PersonId == response.PersonId;

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            PersonId = PersonId,
            Name = Name,
            Email = Email,
            Address = Address,
            CountryId = CountryId,
            DateOfBirth = DateOfBirth,
            Gender = Enum.Parse<GenderOptions>(Gender),
            RecieveNewsLetters = RecieveNewsLetters
        };
    }
}
