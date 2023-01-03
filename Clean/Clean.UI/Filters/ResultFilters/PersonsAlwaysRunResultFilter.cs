using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters.ResultFilters;

public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
{

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Filters.OfType<SkipFilter>().Any())
        {
            return;
        }

        // logic..
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        
    } 
}
