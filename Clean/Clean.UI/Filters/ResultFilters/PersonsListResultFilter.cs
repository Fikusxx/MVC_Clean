using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters.ResultFilters;

public class PersonsListResultFilter : IAsyncResultFilter
{
    private readonly ILogger<PersonsListResultFilter> logger;

    public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
    {
        this.logger = logger;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        logger.LogInformation("{FilterName}.{MethodName} - before method",
            nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

        await next();

        logger.LogInformation("{FilterName}.{MethodName} - after method",
            nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

        context.HttpContext.Response.Headers.Add("ResultFilter", "Value");
    }
}
