using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO.Users;

public class Register
{
    [Required]
    public required string Name { get; set; }
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }
    [Required]
    [RegularExpression("^01[0125][0-9]{8}$", ErrorMessage = "Please provide valid phone number")]
    [DataType(DataType.PhoneNumber)]
    public required string Phone { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public required string ConfirmPassword { get; set; }
}
