using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.DTO.Countries.Response;

public class CountryResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        CountryResponse? countryResponse = obj as CountryResponse;

        if (countryResponse is null) return false;

        if (countryResponse.Name is null) return false;

        return countryResponse.Name.Equals(Name) && countryResponse.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        return Name?.GetHashCode() ?? 0;
    }
}

public static class CountryResponseExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse() { Id = country.Id, Name = country.Name };
    }
}
