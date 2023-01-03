using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters.AuthorizationFilters;

public class TokenAuthorizationFilter : IAuthorizationFilter
{
    private readonly ILogger<TokenAuthorizationFilter> logger;

	public TokenAuthorizationFilter(ILogger<TokenAuthorizationFilter> logger)
	{
		this.logger = logger;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
        bool hasAuthCookie = context.HttpContext.Request.Cookies.ContainsKey("Auth-Key");

        if (hasAuthCookie == false)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        bool hasValidCookie = context.HttpContext.Request.Cookies["Auth-Key"] == "A100";

        if (hasValidCookie == false)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }
    }
}
