using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.AuthorizationFilters;

public class CookieAuthenticationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Filters.OfType<SkipAuthorizationFilter>().Any()) return;

        string? authCookie = GetAuthenticationCookie(context.HttpContext);

        if (authCookie is null || !authCookie.Equals("Pass"))
            context.Result = new UnauthorizedResult();
    }
    private string? GetAuthenticationCookie(HttpContext context)
    {
        bool isAuthCookieApplied = context.Request.Cookies.TryGetValue("Auth-Key", out string? authCookie);

        if (!isAuthCookieApplied) return null;
        if (string.IsNullOrWhiteSpace(authCookie)) return null;

        return authCookie;
    }
}
