
namespace Clean.Core.DTO.CountryDTO;


public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is CountryResponse response)
            return CountryId == response.CountryId;

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
