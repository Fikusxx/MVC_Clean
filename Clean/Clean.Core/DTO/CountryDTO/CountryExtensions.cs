using Clean.Core.Domain.Entities;


namespace Clean.Core.DTO.CountryDTO;


public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryId = country.CountryId,
            Name = country.Name
        };
    }
}
