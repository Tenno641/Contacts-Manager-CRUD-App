using AutoFixture;
using ContactsManager.Core.DTO.Persons;
using ContactsManager.Core.DTO.Persons.Response;
using ContactsManager.Core.ServiceContracts.Countries;
using ContactsManager.Core.ServiceContracts.Persons;
using ContactsManager.Core.Services.Persons;
using ContactsManager.Web.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace ContactsManager.ControllerTests;

public class HomeControllerTests
{
    private readonly ITestOutputHelper _testOutput;
    private readonly Fixture _fixture;
    private readonly Mock<IPersonsAddService> _personsAddServiceMock; 
    private readonly Mock<IPersonsDeleteService> _personsDeleteServiceMock; 
    private readonly Mock<IPersonsGetService> _personsGetServiceMock; 
    private readonly Mock<IPersonsUpdateService> _personsUpdateServiceMock; 
    private readonly HomeController _homeController;
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    public HomeControllerTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _fixture = new Fixture();

        Mock<ICountriesService> countriesServiceMock = new Mock<ICountriesService>();

        _loggerMock = new Mock<ILogger<HomeController>>();

        _personsAddServiceMock = new Mock<IPersonsAddService>();
        _personsDeleteServiceMock = new Mock<IPersonsDeleteService>();
        _personsGetServiceMock = new Mock<IPersonsGetService>();
        _personsUpdateServiceMock = new Mock<IPersonsUpdateService>();

        _homeController = new HomeController(countriesServiceMock.Object, _loggerMock.Object, _personsAddServiceMock.Object, _personsUpdateServiceMock.Object, _personsDeleteServiceMock.Object, _personsGetServiceMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_OkResponse()
    {
        // Arrange
        IEnumerable<PersonResponse> persons = _fixture
            .Build<PersonResponse>()
            .CreateMany();

       _personsGetServiceMock 
            .Setup(service => service.FilterAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(persons);

        _personsGetServiceMock
            .Setup(service => service.OrderAsync(It.IsAny<IEnumerable<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(persons);

        // Act
        IActionResult actionResult = await _homeController.Index(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>());

        // Assert
        actionResult.Should().BeOfType<ViewResult>();

        ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
        viewResult.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.Model.Should().Be(persons);
    }
}
