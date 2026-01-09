using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.DTO.Persons;
using System.ComponentModel.DataAnnotations;

public class PersonRequest
{
    [Required]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public GenderOptions? Gender { get; set; }

    [Required]
    public Guid? CountryId { get; set; }

    [Required]
    public string? Address { get; set; }

    public bool ReceiveNewsLetter { get; set; }

    public Person ToPerson()
    {
        return new Person
        {
            Name = this.Name,
            Email = this.Email,
            DateOfBirth = this.DateOfBirth,
            Gender = this.Gender.ToString(),
            CountryId = this.CountryId,
            Address = this.Address,
            ReceiveNewsLetter = this.ReceiveNewsLetter
        };
    }
}
