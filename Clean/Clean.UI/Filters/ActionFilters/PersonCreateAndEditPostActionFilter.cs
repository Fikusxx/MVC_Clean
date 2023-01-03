using Clean.Core.ServiceContracts;
using Clean.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Stonks.Filters.ActionFilters;

public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
{
    private readonly ICountryService countryService;

    public PersonCreateAndEditPostActionFilter(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is PersonController controller && controller.ModelState.IsValid == false)
        {
            var countries = await countryService.GetAllCountriesAsync();

            controller.ViewBag.Countries =
                countries.Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() });

            controller.ViewBag.Errors =
                controller.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

            var model = context.ActionArguments["model"];
            context.Result = controller.View(model);
        }
        else
        {
            await next();
        }

        // no code here..
    }
}
