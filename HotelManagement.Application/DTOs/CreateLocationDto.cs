using System.ComponentModel.DataAnnotations;

public class CreateLocationDto
{
    [Required, MaxLength(100)]
    public string CityName { get; set; }

    [Required, MaxLength(10)]
    public string CityCode { get; set; }

    [Required]
    public long CountryId { get; set; }
}
