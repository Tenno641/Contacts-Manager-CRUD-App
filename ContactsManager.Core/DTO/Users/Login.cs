using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO.Users;

public class Login
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
