using Clean.Core.DTO.PersonDTO;
using Clean.Core.Enums;
using Clean.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stonks.Filters.ActionFilters;
using Stonks.Filters.AuthorizationFilters;
using Stonks.Filters.ExceptionFilters;
using Stonks.Filters.ResourceFilters;
using Stonks.Filters.ResultFilters;

namespace Clean.UI.Controllers;

[TypeFilter(typeof(HandleExceptionFilter))]
public class PersonController : Controller
{
    private readonly ICountryService countryService;
    private readonly IPersonGetterService personGetterService;
    private readonly IPersonAdderService personAdderService;
    private readonly IPersonUpdaterService personUpdaterService;
    private readonly IPersonDeleterService personDeleterService;
    private readonly IPersonSorterService personSorterService;

    public PersonController(ICountryService countryService, IPersonGetterService personGetterService,
      IPersonAdderService personAdderService, IPersonUpdaterService personUpdaterService,
      IPersonDeleterService personDeleterService, IPersonSorterService personSorterService)
    {
        this.countryService = countryService;
        this.personGetterService = personGetterService;
        this.personAdderService = personAdderService;
        this.personUpdaterService = personUpdaterService;
        this.personDeleterService = personDeleterService;
        this.personSorterService = personSorterService;
    }

    [HttpGet]
    [Route("/")]
    [Route("[controller]/[action]")]
    [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
    [TypeFilter(typeof(PersonsListResultFilter))]
    [ResponseHeaderFilterFactory(key: "key", value: "value", order: 1)]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        List<PersonResponse> persons = await personGetterService.GetFilteredPersonsAsync(searchBy, searchString);
        List<PersonResponse> sortedPersons = await personSorterService.GetSortedPersonsAsync(persons, sortBy, sortOrder);

        return View(sortedPersons);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var countries = await countryService.GetAllCountriesAsync();
        ViewBag.Countries = countries.Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() });

        return View();
    }

    [HttpPost]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    [TypeFilter(typeof(FeatureDisabledResourceFilter))]
    public async Task<IActionResult> Create(PersonAddRequest model)
    {
        //if (ModelState.IsValid == false)
        //{
        //    var countries = await countryService.GetAllCountriesAsync();
        //    ViewBag.Countries = countries.Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() });
        //    ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

        //    return View(model);
        //}

        await personAdderService.AddPersonAsync(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [TypeFilter(typeof(TokenResultFilter))]
    public async Task<IActionResult> Edit(Guid id)
    {
        var person = await personGetterService.GetPersonByIdAsync(id);

        if (person == null)
            return NotFound();

        var personUpdateRequest = person.ToPersonUpdateRequest();
        var countries = await countryService.GetAllCountriesAsync();
        var countriesSelectList = countries.Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() });
        ViewBag.Countries = countriesSelectList;

        return View(personUpdateRequest);
    }

    [HttpPost]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    [TypeFilter(typeof(TokenAuthorizationFilter))]
    public async Task<IActionResult> Edit(PersonUpdateRequest model)
    {
        //if (ModelState.IsValid == false)
        //{
        //    var countries = await countryService.GetAllCountriesAsync();
        //    var countriesSelectList = countries.Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() });
        //    ViewBag.Countries = countriesSelectList;

        //    return View(model);
        //}

        await personUpdaterService.UpdatePersonAsync(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var personResponse = await personGetterService.GetPersonByIdAsync(id);

        if (personResponse == null)
            return NotFound();

        return View(personResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(PersonResponse person)
    {
        await personDeleterService.DeletePersonAsync(person.PersonId);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> PersonsCSV()
    {
        var memoryStream = await personGetterService.GetPersonsCSV();

        return File(memoryStream, "application/octet-stream", "persons.csv");
    }

    public async Task<IActionResult> PersonsExcel()
    {
        var memoryStream = await personGetterService.GetPersonsExcel();

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
    }

    [HttpGet]
    public IActionResult UploadExcelFile()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadExcelFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.ErrorMessage = "Please upload a file";
            return View();
        }

        var isValidExtension = Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase);

        if (isValidExtension == false)
        {
            ViewBag.ErrorMessage = "Unsupported file format";
            return View();
        }

        int addedCountries = await countryService.UploadFromExcelFile(file);
        ViewBag.TotalCountries = addedCountries;

        return View();
    }
}
