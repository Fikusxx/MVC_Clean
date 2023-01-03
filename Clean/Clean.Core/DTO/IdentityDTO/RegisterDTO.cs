using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clean.Core.DTO.IdentityDTO;


public class RegisterDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [Remote(action: "IsEmailInUse", controller: "Account")]
    public string Email { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
