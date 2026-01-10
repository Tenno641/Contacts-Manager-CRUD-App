using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Domain.Entities;

public class Person
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [MaxLength(40)]
    public string? Name { get; set; }
    [MaxLength(40)]
    public string? Email { get; set;}
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(40)]
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
    [MaxLength(40)]
    public string? Address { get; set; }
    public bool ReceiveNewsLetter { get; set; }
}
