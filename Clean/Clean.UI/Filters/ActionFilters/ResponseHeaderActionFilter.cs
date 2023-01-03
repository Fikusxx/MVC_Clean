using Microsoft.AspNetCore.Mvc.Filters;


namespace Stonks.Filters.ActionFilters;

public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
{
    private readonly ILogger<ResponseHeaderActionFilter> logger;
    public int Order { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }

    public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
    {
        this.logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("{FilterName}.{MethodName} - before method",
            nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

        await next();

        logger.LogInformation("{FilterName}.{MethodName} - after method",
            nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
    }
}

public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;
    private readonly int order;
    private readonly string key;
    private readonly string value;

    public ResponseHeaderFilterFactoryAttribute(int order, string key, string value)
    {
        this.order = order;
        this.key = key;
        this.value = value;
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
        filter.Key = key;
        filter.Value = value;
        filter.Order = order;

        return filter;
    }
}