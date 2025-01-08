using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyPlanner.Models;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(MobilePhone), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MinLength(10), MaxLength(30)]
    //precisa ser unique
    public string Username { get; set; }
    [Required]
    [Phone]
    //precisa ser unique
    public string MobilePhone { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}