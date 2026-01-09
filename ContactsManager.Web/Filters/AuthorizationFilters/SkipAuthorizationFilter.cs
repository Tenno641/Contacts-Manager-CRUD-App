using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.AuthorizationFilters;

public class SkipAuthorizationFilter : Attribute, IFilterMetadata
{

}
