using Microsoft.AspNetCore.Mvc.Filters;

namespace Stonks.Filters;

public class SkipFilter : Attribute, IFilterMetadata
{
}
