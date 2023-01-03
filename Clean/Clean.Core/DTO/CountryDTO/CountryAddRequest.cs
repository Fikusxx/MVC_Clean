using Clean.Core.Domain.Entities;


namespace Clean.Core.DTO.CountryDTO;


public class CountryAddRequest
{
    public string Name { get; set; }

    public Country ConvertToCountry()
    {
        return new Country() { Name = Name };
    }
}
