using Clean.Core.Domain.Entities;
using Clean.Core.DTO.CountryDTO;
using Clean.Core.ServiceContracts;
using Clean.Core.Services;
using Clean.Infrastructure.Database;
using Clean.Infrastructure.Repository;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;

namespace Clean.ServiceTests;

public class CountryServiceTest
{
    private readonly ICountryService countryService;

    public CountryServiceTest()
    {
        // empty list to be filled with data
        var countriesList = new List<Country>();

        // dbOptions to pass to AppDbContext ctor
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

        // create db mock based on real AppDbContext passing dbOptions in ctor
        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(dbOptions);

        // get a copy of AppDbContext
        ApplicationDbContext db = dbContextMock.Object;

        // fill empty list with DbSet<T> from AppDbContext
        dbContextMock.CreateDbSetMock(x => x.Countries, countriesList);

        // init the service with mock (fake) db
        countryService = new CountryService(new CountryRepository(db));
    }

    #region Add Country

    [Fact]
    public async Task AddCountry_NullCountry()
    {
        // Arrange 
        CountryAddRequest? request = null;

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await countryService.AddCountryAsync(request));
    }

    [Fact]
    public async Task AddCountry_NullCountryName()
    {
        // Arrange 
        CountryAddRequest? request = new CountryAddRequest() { Name = null };

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await countryService.AddCountryAsync(request));
    }

    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        // Arrange 
        CountryAddRequest? request1 = new CountryAddRequest() { Name = "USA" };
        CountryAddRequest? request2 = new CountryAddRequest() { Name = "USA" };

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
           await countryService.AddCountryAsync(request1);
           await countryService.AddCountryAsync(request2);
        });
    }

    [Fact]
    public async Task AddCountry_ProperDetails()
    {
        // Arrange 
        CountryAddRequest? request = new CountryAddRequest() { Name = "Japan" };

        // Act
        var countryResponse = await countryService.AddCountryAsync(request);
        var responseList = await countryService.GetAllCountriesAsync();

        // Assert
        Assert.True(countryResponse.CountryId != Guid.Empty);
        Assert.Contains(countryResponse, responseList);
    }

    #endregion

    #region Get All Countries

    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        var list = await countryService.GetAllCountriesAsync();

        Assert.Empty(list);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        var list = new List<CountryAddRequest>()
        {
            new CountryAddRequest() { Name = "USA"},
            new CountryAddRequest() { Name = "Japan"},
            new CountryAddRequest() { Name = "UK"},
        };

        var responseList = new List<CountryResponse>();

        foreach (var item in list)
        {
            responseList.Add(await countryService.AddCountryAsync(item));
        }

        var actualResponseList = await countryService.GetAllCountriesAsync();

        foreach (var item in responseList)
        {
            Assert.Contains(item, actualResponseList);
        }
    }

    #endregion

    #region Get Country By Id

    [Fact]
    public async Task GetCountry_NullCountryId()
    {
        var response = await countryService.GetCountryByIdAsync(null);

        Assert.Null(response);
    }

    [Fact]
    public async Task GetCountry_ValidCountryId()
    {
        var countryAddRequest = new CountryAddRequest() { Name = "USA" };
        var countryResponse = await countryService.AddCountryAsync(countryAddRequest);

        var response = await countryService.GetCountryByIdAsync(countryResponse.CountryId);

        Assert.Equal(response, countryResponse);
    }

    #endregion
}
