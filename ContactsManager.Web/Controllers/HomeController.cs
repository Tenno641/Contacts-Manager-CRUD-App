using ContactsManager.Core.DTO.Persons;
using ContactsManager.Core.DTO.Persons.Request;
using ContactsManager.Core.DTO.Persons.Response;
using ContactsManager.Core.ServiceContracts.Countries;
using ContactsManager.Core.ServiceContracts.Persons;
using ContactsManager.Web.Filters.AuthorizationFilters;
using ContactsManager.Web.Filters.ActionFilters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativaio.AspNetCore;
using Microsoft.AspNetCore.Authorization;

namespace ContactsManager.Web.Controllers;
[Route("[Controller]")]
//[TypeFilter(typeof(CookieAuthenticationFilter))]
public class HomeController : Controller
{
    private readonly IPersonsDeleteService _personsDeleteService;
    private readonly IPersonsAddService _personsAddService;
    private readonly IPersonsUpdateService _personsUpdateService;
    private readonly IPersonsGetService _personsGetService;
    private readonly ICountriesService _countriesService;
    private readonly ILogger<HomeController> _logger;
    public HomeController(
        ICountriesService countriesService,
        ILogger<HomeController> logger,
        IPersonsAddService personsAddService,
        IPersonsUpdateService personsUpdateService,
        IPersonsDeleteService personsDeleteService,
        IPersonsGetService personsGetService
        )
    {
        _countriesService = countriesService;
        _logger = logger;
        _personsAddService = personsAddService;
        _personsUpdateService = personsUpdateService;
        _personsDeleteService = personsDeleteService;
        _personsGetService = personsGetService;
    }

    [Route("[action]")]
    [TypeFilter(typeof(CustomActionFilters))]
    [SkipAuthorizationFilter]
    public async Task<IActionResult> Index(string searchBy, string searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.Ascending)
    {
        _logger.LogInformation("Okay It's the home controller");
        ViewBag.SearchOptions = new Dictionary<string, string>() {
            { "Name", "Person Name"}, {"Email", "Email"}, {"DateOfBirth","Date Of Birth" }, {"Age", "Age"},{"Gender","Gender"}, {"CountryName", "Country"},{"Address","Address"}, {"ReceiveNewsLetters", "Receive News Letters" }
        };

        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        IEnumerable<PersonResponse> filteredPersons = await _personsGetService.FilterAsync(searchBy, searchString);

        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;

        IEnumerable<PersonResponse> sortedPersons = await _personsGetService.OrderAsync(filteredPersons, sortBy, sortOrder);

        return View(sortedPersons);
    }

    [Route("get-cookie")]
    [HttpGet]
    [SkipAuthorizationFilter]
    public IActionResult Authenticate()
    {
        HttpContext.Response.Cookies.Append("Auth-Key", "Pass");
        return Ok("Yum");
    }

    [Route("/")]
    public IActionResult Redirect()
    {
        return RedirectToAction(nameof(Index), "Home");
    }

    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> CreatePerson()
    {
        ViewBag.Countries = await GetCountriesListItemsAsync();
        return View();
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> CreatePerson(PersonRequest personRequest)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = GetCountriesListItemsAsync();
            ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();
            return View();
        }
        await _personsAddService.AddPersonAsync(personRequest);
        return RedirectToAction("Index", "Home");
    }

    [Route("[action]/{id:guid}")]
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        PersonResponse? person = await _personsGetService.GetAsync(id);
        if (person is null)
        {
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
        IEnumerable<SelectListItem> countries = await GetCountriesListItemsAsync();
        ViewBag.Countries = countries;

        PersonUpdateRequest personUpdate = person.Value.ToPersonUpdateRequest();
        return View(personUpdate);
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Update(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? person = await _personsGetService.GetAsync(personUpdateRequest.Id);
        if (person is null)
        {
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        if (!ModelState.IsValid)
        {
            IEnumerable<SelectListItem> countries = await GetCountriesListItemsAsync();
            ViewBag.Countries = countries;
            ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage);
            ModelState.Clear();
            return View(person.Value.ToPersonUpdateRequest());
        }

        await _personsUpdateService.UpdateAsync(personUpdateRequest);
        return RedirectToAction(actionName: "Index", controllerName: "Home");
    }

    [Route("[action]/{id:guid}")]
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        PersonResponse? person = await _personsGetService.GetAsync(id);
        if (person is null)
        {
            return RedirectToAction(actionName: "Index");
        }
        return View(person);
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
    {
        await _personsDeleteService.DeleteAsync(personUpdateRequest.Id);
        return RedirectToAction(actionName: "Index");
    }
    [Route("PersonsPdf")]
    public async Task<IActionResult> PersonsPdf()
    {
        IEnumerable<PersonResponse> persons = await _personsGetService.GetAllAsync();
        return new ViewAsPdf(persons)
        {
            PageOrientation = Orientation.Landscape
        };
    }

    [Route("PersonsCsv")]
    public async Task<IActionResult> PersonsCsv()
    {
        MemoryStream memoryStream = await _personsGetService.GetPersonsCsvAsync();
        return File(memoryStream, "text/csv");
    }
    [Route("PersonsExcel")]
    public async Task<IActionResult> PersonsExcel()
    {
        MemoryStream stream = await _personsGetService.GetPersonsExcelAsync();
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx");
    }
    private async Task<IEnumerable<SelectListItem>> GetCountriesListItemsAsync()
    {
        return (await _countriesService.GetAllAsync()).Select(country => new SelectListItem() { Text = country.Name, Value = country.Id.ToString() });
    }

    [AllowAnonymous]
    [Route("/Error")]
    public IActionResult Error()
    {
        IExceptionHandlerFeature? exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature is null) return View();
        ViewBag.ErrorMessage = exceptionFeature.Error.Message;
        ViewBag.ErrorType = exceptionFeature.Error.GetType();
        return View();
    }
}
