using ContactsManager.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO.Persons.Request;

public class PersonUpdateRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetter { get; set; }

    public Person ToPerson()
    {
        return new Person
        {
            Id = Id,
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender?.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetter = ReceiveNewsLetter
        };
    }
}

