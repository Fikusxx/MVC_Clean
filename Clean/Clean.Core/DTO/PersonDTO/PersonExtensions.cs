using Clean.Core.Domain.Entities;

namespace Clean.Core.DTO.PersonDTO;


public static class PersonExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        int? age = null;

        if (person.DateOfBirth != null)
            age = DateTime.Now.Year - person.DateOfBirth.Value.Year;

        return new PersonResponse()
        {
            PersonId = person.PersonId,
            Name = person.Name,
            Email = person.Email,
            Address = person.Address,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            RecieveNewsLetters = person.RecieveNewsLetters,
            CountryId = person.CountryId,
            Age = age,
            CountryName = person.Country?.Name
        };
    }
}
