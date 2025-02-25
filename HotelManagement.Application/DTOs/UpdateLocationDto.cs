using System.ComponentModel.DataAnnotations;

public class UpdateLocationDto
{
    [Required, MaxLength(100)]
    public string CityName { get; set; }

    [Required, MaxLength(10)]
    public string CityCode { get; set; }
}
