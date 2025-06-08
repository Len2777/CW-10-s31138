using System.ComponentModel.DataAnnotations;

namespace CW10.DTOs;

public class AssignClientDto
{
    [MaxLength(120)] public string FirstName { get; set; } = null!;
    [MaxLength(120)] public string LastName { get; set; } = null!;
    [MaxLength(120)] public string Email { get; set; } = null!;
    [MaxLength(120)] public string Telephone { get; set; } = null!;
    [MaxLength(120)] public string Pesel { get; set; } = null!;
    public DateTime? PaymentDate { get; set; }
}