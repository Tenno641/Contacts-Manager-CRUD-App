using ContactsManager.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO.Countries.Request;
public class CountryRequest
{
    [Required]
    public string? Name { get; init; }
    public Country ToCountry()
    {
        return new Country() { Name = Name };
    }
}
