using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters.ResourceFilters;

public class FeatureDisabledResourceFilter : IAsyncResourceFilter
{
    private readonly ILogger<FeatureDisabledResourceFilter> logger;
    private readonly bool isDisabled;

    public FeatureDisabledResourceFilter(
        ILogger<FeatureDisabledResourceFilter> logger, bool isDisabled = true)
    {
        this.logger = logger;
        this.isDisabled = isDisabled;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        logger.LogInformation("{FilterName}.{MethodName} - before method",
            nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));

        if (isDisabled)
            context.Result = new NotFoundResult();
        else
            await next();

        logger.LogInformation("{FilterName}.{MethodName} - after method",
            nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
    }
}
