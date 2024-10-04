using System.ComponentModel.DataAnnotations;

namespace Cityinfo.API.Models;

public class RegisterRequest
{
    [Required] public string UserName { get; set; } = string.Empty;
    [Required] [MinLength(8)] public string Password { get; set; } = string.Empty;
}
