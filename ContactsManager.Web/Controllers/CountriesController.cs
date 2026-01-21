using ContactsManager.Core.ServiceContracts.Countries;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers;

[Route("[controller]")]
public class CountriesController : Controller
{
    private readonly ICountriesService _countriesService;
    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }
    [Route("ImportExcelFile")]
    [HttpGet]
    public IActionResult ImportExcelFile()
    {
        return View();
    }
    [Route("ImportExcelFile")]
    [HttpPost]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
        if (file is null || file.Length < 0)
        {
            ViewBag.ErrorMessage = "submitted file can't be empty";
            return View();
        }
        Console.WriteLine(file.Name);
        Console.WriteLine(file.FileName);
        if (file.FileName.Contains(".xlsx", StringComparison.InvariantCultureIgnoreCase))
        {
            ViewBag.ErrorMessage = "Please provide excel file (xlsx)";
        }

        int affectedRows = await _countriesService.ImportFromExcelFile(file);
        ViewBag.Message = $"{affectedRows} countries added!";
        return View();
    }
}
