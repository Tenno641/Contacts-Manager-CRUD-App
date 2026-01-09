using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Domain.Entities;

public class Country
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(40)]
    public string? Name { get; set; }
    public ICollection<Person>? Persons { get; set; }
}
