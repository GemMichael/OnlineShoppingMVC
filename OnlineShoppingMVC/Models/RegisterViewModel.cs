using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    public required string Address { get; set; }

    [Required]
    public required string Phone { get; set; }
}
