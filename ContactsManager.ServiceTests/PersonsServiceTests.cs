using AutoFixture;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO.Persons;
using ContactsManager.Core.DTO.Persons.Response;
using ContactsManager.Core.ServiceContracts.Persons;
using ContactsManager.Core.Services.Persons;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace ContactsManager.ServiceTests;

public class PersonsServiceTests
{
    private readonly IPersonsAddService _personsAddService;
    private readonly IPersonsGetService _personsGetService;
    private readonly Fixture _fixture;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly ITestOutputHelper _testOutput;
    public PersonsServiceTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _fixture = new Fixture();

        _personsRepositoryMock = new Mock<IPersonsRepository>();

        _personsAddService = new PersonsAddService(_personsRepositoryMock.Object);
        _personsGetService = new PersonsGetService(_personsRepositoryMock.Object);
    }

    #region Add Person
    [Fact]
    public async Task AddPerson_ThrowsArgumentNull_ArgumentIsNull()
    {
        PersonRequest? person = null;

        // Act
        Func<Task> action = () => _personsAddService.AddPersonAsync(person!);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddPerson_ThrowsArgumentException_IfPropertyIsNull()
    {
        // Arrange
        PersonRequest personRequest = new();

        // Act
        Func<Task> action = () => _personsAddService.AddPersonAsync(personRequest);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_PersonAdded_IfPersonIsValid()
    {
        // Arrange 
        PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(person => person.Email, "Example@Example.con")
            .Create();

        Person person = personRequest.ToPerson();
        PersonResponse expected = PersonResponseExtension.ToPersonResponse.Compile().Invoke(person);

        _personsRepositoryMock
            .Setup(repo => repo.AddPersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(person);

        // Act
        PersonResponse response = await _personsAddService.AddPersonAsync(personRequest);
        expected.Id = response.Id;

        // Assert
        response.Id.Should().NotBe(Guid.Empty);
        expected.Should().Be(response);
    }
    #endregion

    #region Get Person
    [Fact]
    public async Task Get_ReturnsNull_IfIdIsNull()
    {
        Guid? id = null;
        PersonResponse? response = await _personsGetService.GetAsync(id);
        Assert.Null(response);
    }

    [Fact]
    public async Task Get_ValidPersonResponseObject_ProvidingValidId()
    {
        // Arrange
        Person person = _fixture.Build<Person>().With(person => person.Country, null as Country).Create();

        _personsRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        // Act
        PersonResponse? actualResponse = await _personsGetService.GetAsync(person.Id);
        PersonResponse expected = PersonResponseExtension.ToPersonResponse.Compile().Invoke(person);

        // Assert
        actualResponse.Should().Be(expected);
    }
    #endregion

    #region GetAll
    [Fact]
    public async Task GetAll_ReturnsEmptyList_NoAddedPersons()
    {
        // Arrange
        _personsRepositoryMock
            .Setup(repo => repo.AllAsync())
            .ReturnsAsync([]);

        // Assert
        (await _personsGetService.GetAllAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnPersons_IfWeAddedValidPersons()
    {
        // Arrange
        IEnumerable<Person> persons = _fixture.Build<Person>().With(person => person.Country, null as Country).CreateMany();

        _personsRepositoryMock
            .Setup(repo => repo.AllAsync())
            .ReturnsAsync(persons);

        // Act
        IEnumerable<PersonResponse> actualPersonResponses = await _personsGetService.GetAllAsync();
        IEnumerable<PersonResponse> expectedPersonResponses = persons.Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person));

        // Assert
        actualPersonResponses.Should().BeEquivalentTo(expectedPersonResponses);
    }

    #endregion

    #region Filter
    [Fact]
    public async Task Filter_GetFilteredPersons()
    {
        // Arrange
        IEnumerable<Person> persons =
            [
                _fixture.Build<Person>().With(person => person.Name, "Mamdani").With(person => person.Country, null as Country).Create(),
            _fixture.Build<Person>().With(person => person.Name, "Rufus").With(person => person.Country, null as Country).Create(),
            _fixture.Build<Person>().With(person => person.Name, "motor").With(person => person.Country, null as Country).Create(),
        ];

        _personsRepositoryMock
            .Setup(repo => repo.FilterAsync(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons.Where(person => person.Name == null || person.Name.Contains('m')));

        // Act
        IEnumerable<PersonResponse> actualResponses = await _personsGetService.FilterAsync("Name", "m");

        IEnumerable<PersonResponse> expectedResponses = persons.Where(person => person.Name == null || person.Name.Contains('m')).Select(PersonResponseExtension.ToPersonResponse.Compile());

        _testOutput.WriteLine("actual:");
        _testOutput.WriteLine(string.Join("\n", actualResponses));
        _testOutput.WriteLine("Expected");
        _testOutput.WriteLine(string.Join("\n", expectedResponses));

        // Assert
        actualResponses.Should().BeEquivalentTo(expectedResponses);
    }
    #endregion

    #region Order
    [Fact]
    public async Task OrderBy_ReturnsSortedPersonsList()
    {
        // Arrange
        IEnumerable<Person> persons = _fixture.Build<Person>()
            .With(person => person.Country, null as Country)
            .CreateMany();

        _personsRepositoryMock
            .Setup(repo => repo.AllAsync())
            .ReturnsAsync(persons);

        // Act
        IEnumerable<PersonResponse> expectedPersons = persons.OrderBy(person => person.Name).Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person));

        IEnumerable<PersonResponse> actualPersons = await _personsGetService.OrderAsync(persons.Select(person => PersonResponseExtension.ToPersonResponse.Compile().Invoke(person)), nameof(PersonResponse.Name), SortOrderOptions.Ascending);

        _testOutput.WriteLine("actual:");
        _testOutput.WriteLine(string.Join("\n", actualPersons));
        _testOutput.WriteLine("Expected");
        _testOutput.WriteLine(string.Join("\n", expectedPersons));

        // Assert
        actualPersons.Should().BeEquivalentTo(expectedPersons);
    }
    #endregion
}