using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.DTO.Persons.Request;
using System.Linq.Expressions;

namespace ContactsManager.Core.DTO.Persons.Response;
public record struct PersonResponse
(
    Guid Id,
    string? Name,
    string? Email,
    DateTime? DateOfBirth,
    string? Gender,
    Guid? CountryId,
    string? CountryName,
    int? Age,
    string? Address,
    bool ReceiveNewsLetter
);

public static class PersonResponseExtension
{
    public static Expression<Func<Person, PersonResponse>> ToPersonResponse => person => new PersonResponse()
    {
        Id = person.Id,
        Name = person.Name,
        Email = person.Email,
        DateOfBirth = person.DateOfBirth,
        Gender = person.Gender,
        CountryId = person.CountryId,
        Address = person.Address,
        ReceiveNewsLetter = person.ReceiveNewsLetter,
        Age = CalculateAge(person.DateOfBirth), 
        CountryName = person.Country != null ? person.Country.Name : null
    };

    public static PersonUpdateRequest ToPersonUpdateRequest(this PersonResponse personResponse)
    {
        return new PersonUpdateRequest()
        {
            Id = personResponse.Id,
            Address = personResponse.Address,
            CountryId = personResponse.CountryId,
            DateOfBirth = personResponse.DateOfBirth,
            Email = personResponse.Email,
            Gender = Enum.TryParse(personResponse.Gender, true, out GenderOptions value) ? value : null,
            Name = personResponse.Name,
            ReceiveNewsLetter = personResponse.ReceiveNewsLetter
        };
    }

    private static int? CalculateAge(DateTime? dateOfBirth)
    {
        if (dateOfBirth is null) return null;

        DateTime today = DateTime.UtcNow;

        int age = today.Year - dateOfBirth.Value.Year;

        if (dateOfBirth.Value.AddYears(age) > today)
        {
            age--;
        }

        return age;
    }
}
