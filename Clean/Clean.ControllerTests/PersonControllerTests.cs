using AutoFixture;
using Clean.Core.DTO.CountryDTO;
using Clean.Core.DTO.PersonDTO;
using Clean.Core.Enums;
using Clean.Core.ServiceContracts;
using Clean.UI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;

namespace Clean.ControllerTests;

public class PersonControllerTests
{
    private readonly IPersonGetterService personGetterService;
    private readonly IPersonAdderService personAdderService;
    private readonly IPersonUpdaterService personUpdaterService;
    private readonly IPersonDeleterService personDeleterService;
    private readonly IPersonSorterService personSorterService;
    private readonly ICountryService countryService;

    private readonly Mock<IPersonGetterService> personGetterMock;
    private readonly Mock<IPersonAdderService> personAdderMock;
    private readonly Mock<IPersonUpdaterService> personUpdaterMock;
    private readonly Mock<IPersonDeleterService> personDeleterMock;
    private readonly Mock<IPersonSorterService> personSorterMock;
    private readonly Mock<ICountryService> countryServiceMock;

    private readonly IFixture fixture = new Fixture();
    private readonly PersonController controller;

    public PersonControllerTests()
    {
        personGetterMock = new Mock<IPersonGetterService>();
        personAdderMock = new Mock<IPersonAdderService>();
        personUpdaterMock = new Mock<IPersonUpdaterService>();
        personDeleterMock = new Mock<IPersonDeleterService>();
        personSorterMock = new Mock<IPersonSorterService>();
        countryServiceMock = new Mock<ICountryService>();

        personGetterService = personGetterMock.Object;
        personAdderService = personAdderMock.Object;
        personUpdaterService = personUpdaterMock.Object;
        personDeleterService = personDeleterMock.Object;
        personSorterService = personSorterMock.Object;
        countryService = countryServiceMock.Object;

        controller = new PersonController(countryService, personGetterService, personAdderService,
            personUpdaterService, personDeleterService, personSorterService);
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnViewWithPersonsList()
    {
        List<PersonResponse> personsList = fixture.Create<List<PersonResponse>>();

        personGetterMock.Setup(x => x.GetFilteredPersonsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(personsList);
        personSorterMock.Setup(x => x.GetSortedPersonsAsync(It.IsAny<List<PersonResponse>>(),
            It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personsList);

        var result = await controller.Index(fixture.Create<string>(), fixture.Create<string>(),
              fixture.Create<string>(), fixture.Create<SortOrderOptions>());

        // view itself
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        // exact model objects passed to the view
        viewResult.ViewData.Model.Should().Be(personsList);
        // exact model type passed to the view
        viewResult.ViewData.Model.Should().BeOfType<List<PersonResponse>>();
        // model can be a collection of any IEnumerable type
        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
    }

    #endregion

    #region Create POST

    [Fact]
    public async Task Create_IfModelErrors_ToReturnView()
    {
        PersonAddRequest personAddRequest = fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = fixture.Create<PersonResponse>();
        List<CountryResponse> countryResponses = fixture.Create<List<CountryResponse>>();

        countryServiceMock.Setup(x => x.GetAllCountriesAsync())
            .ReturnsAsync(countryResponses);

        // Manually set a model state error, so validation fails
        controller.ModelState.AddModelError("Name", "Name cant be empty");

        // call controllers method
        var result = await controller.Create(personAddRequest);

        // return type itself
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        // exact model objects passed to the view
        viewResult.ViewData.Model.Should().Be(personAddRequest);
        // exact model type passed to the view
        viewResult.ViewData.Model.Should().BeOfType<PersonAddRequest>();
        // model can be a collection of any IEnumerable type
        viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

        // View Data checks
        viewResult.ViewData.ContainsKey("Countries").Should().BeTrue();
        viewResult.ViewData.TryGetValue("Countries", out object? value);
        value.Should().NotBeNull();
        value.Should().BeAssignableTo<IEnumerable<SelectListItem>>();
    }

    [Fact]
    public async Task Create_IfNoModelErrors_ToReturnRedirectToAction()
    {
        PersonAddRequest personAddRequest = fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = fixture.Create<PersonResponse>();

        personAdderMock.Setup(x => x.AddPersonAsync(It.IsAny<PersonAddRequest>()))
           .ReturnsAsync(personResponse);

        // call controllers method
        var result = await controller.Create(personAddRequest);

        // return type itself
        var viewResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        // method we redirect to
        viewResult.ActionName.Should().Be("Index");
    }

    #endregion
}