using Clean.Core.Domain.Entities;
using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.DTO.CountryDTO;
using Clean.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Clean.Core.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        this.countryRepository = countryRepository;
    }

    public async Task<CountryResponse> AddCountryAsync(CountryAddRequest? request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Name == null)
            throw new ArgumentNullException(nameof(request.Name));

        if (await countryRepository.GetByNameAsync(request.Name) != null)
            throw new ArgumentException("Already exists");


        var country = request.ConvertToCountry();
        country.CountryId = Guid.NewGuid();
        await countryRepository.AddAsync(country);

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountriesAsync()
    {
        var countryList = await countryRepository.GetAllAsync();
        var responseList = countryList.Select(x => x.ToCountryResponse()).ToList();

        return responseList;
    }

    public async Task<CountryResponse?> GetCountryByIdAsync(Guid? countryId)
    {
        var response = await countryRepository.GetByIdAsync(countryId.GetValueOrDefault());

        if (response == null)
            return null;

        return response.ToCountryResponse();
    }

    public async Task<int> UploadFromExcelFile(IFormFile file)
    {
        var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        using (var package = new ExcelPackage(ms))
        {
            // Assuming that worksheet "Countries" exists
            ExcelWorksheet workSheet = package.Workbook.Worksheets["Countries"];

            int rowCount = workSheet.Dimension.Rows; // number of rows from top to bottom
            int addedCountries = 0;

            // 1st column being Name
            for (int row = 2; row <= rowCount; row++) // row = 1 being the header, so we start from 2
            {
                string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                if (string.IsNullOrWhiteSpace(cellValue) == false)
                {
                    string countryName = cellValue;

                    // if there's no object with the same parameter - then add it
                    if (await countryRepository.GetByNameAsync(countryName) == null)
                    {
                        var country = new Country()
                        {
                            Name = countryName
                        };
                        await countryRepository.AddAsync(country);
                        addedCountries++;
                    };
                }
            }

            return addedCountries;
        }
    }
}
