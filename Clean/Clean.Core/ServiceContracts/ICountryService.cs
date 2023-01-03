using Clean.Core.DTO.CountryDTO;
using Microsoft.AspNetCore.Http;

namespace Clean.Core.ServiceContracts;


public interface ICountryService
{
    public Task<CountryResponse> AddCountryAsync(CountryAddRequest? request);
    public Task<List<CountryResponse>> GetAllCountriesAsync();
    public Task<CountryResponse?> GetCountryByIdAsync(Guid? countryId);
    public Task<int> UploadFromExcelFile(IFormFile file);

}