using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Locations")]
public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string CityName { get; set; }

    [Required, MaxLength(10)]
    public string CityCode { get; set; }

    [Required]
    public long CountryId { get; set; } 

    [ForeignKey("CountryId")]
    public Country Country { get; set; }
}
