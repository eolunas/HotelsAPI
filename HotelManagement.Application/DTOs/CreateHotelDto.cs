using System.ComponentModel.DataAnnotations;

public class CreateHotelDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; }

    [Required(ErrorMessage = "LocationId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid LocationId.")]
    public int LocationId { get; set; }

    public bool IsEnabled { get; set; }
}
