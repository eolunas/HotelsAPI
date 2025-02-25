using System.ComponentModel.DataAnnotations;

public class CreateCountryDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, MaxLength(3), RegularExpression("^[A-Z]{3}$", ErrorMessage = "CountryCode must be exactly 3 uppercase letters.")]
    public string CountryCode { get; set; }
}
