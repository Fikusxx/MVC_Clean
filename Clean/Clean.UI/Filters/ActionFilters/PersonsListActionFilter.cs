using Clean.Core.DTO.PersonDTO;
using Clean.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters.ActionFilters;

public class PersonsListActionFilter : Attribute, IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> logger;

    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.ContainsKey("searchBy"))
        {
            string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

            if (searchBy != null)
            {
                var searchOptions = new List<string>()
                {
                    nameof(PersonResponse.Name),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryId),
                    nameof(PersonResponse.Address)
                };

                if (searchOptions.Any(x => x == searchBy) == false)
                {
                    logger.LogInformation($"searchBy value is {searchBy}");
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);
                }
            }
        }

        context.HttpContext.Items["ActionArguments"] = context.ActionArguments;
        logger.LogInformation("PersonsListActionFilter.OnActionExecuting");
    }

    // Before logic
    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controller = context.Controller as PersonController;

        if (controller == null)
            return;

        var arguments = (IDictionary<string, object?>?)context.HttpContext.Items["ActionArguments"];

        if (arguments == null)
            return;

        if (arguments.ContainsKey("searchBy"))
            controller.ViewData["CurrentSearchBy"] = Convert.ToString(arguments["searchBy"]);

        if (arguments.ContainsKey("searchString"))
            controller.ViewData["CurrentSearchString"] = Convert.ToString(arguments["searchString"]);

        if (arguments.ContainsKey("sortBy"))
            controller.ViewData["CurrentSortBy"] = Convert.ToString(arguments["sortBy"]);

        if (arguments.ContainsKey("sortOrder"))
            controller.ViewData["CurrentSortOrder"] = Convert.ToString(arguments["sortOrder"]);

        controller.ViewBag.SearchFields = new Dictionary<string, string>()
          {
            { nameof(PersonResponse.Name), "Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryName), "Country" },
            { nameof(PersonResponse.Address), "Address" }
          };
    }
}
