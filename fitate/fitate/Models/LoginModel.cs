using System.ComponentModel.DataAnnotations;

namespace fitate.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public int Height { get; set; }
    public bool Gender { get; set; }
    public int Age { get; set; }
}