using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters.ExceptionFilters;

public class HandleExceptionFilter : IExceptionFilter
{
    private readonly ILogger<HandleExceptionFilter> logger;
    private readonly IWebHostEnvironment webHostEnvironment;

    public HandleExceptionFilter(
        ILogger<HandleExceptionFilter> logger, IWebHostEnvironment webHostEnvironment)
    {
        this.logger = logger;
        this.webHostEnvironment = webHostEnvironment;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError("{FilterName}.{MethodName} method",
          nameof(HandleExceptionFilter), nameof(OnException));

        logger.LogError("{ExceptionType}\n{ExceptionMessage}",
            context.Exception.GetType().Name, context.Exception.Message);

        if (webHostEnvironment.IsDevelopment())
        {
            context.Result = new ContentResult() { Content = context.Exception.Message, StatusCode = 501 };
        }
    }
}
