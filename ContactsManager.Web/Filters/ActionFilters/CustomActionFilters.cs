using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.ActionFilters;

public class CustomActionFilters : IAsyncResultFilter
{
    private readonly ILogger<CustomActionFilters> _logger;
    public CustomActionFilters(ILogger<CustomActionFilters> logger)
    {
        _logger = logger;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        _logger.LogInformation("{ResultFilter} OnResultExecuting", nameof(ResultExecutingContext));
        await next();
        _logger.LogInformation("{ResultFilter} OnResultExecuted", nameof(ResultExecutedContext));
    }
}
