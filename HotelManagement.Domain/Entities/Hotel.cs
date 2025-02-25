using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Hotels")]
public class Hotel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public long LocationId { get; set; }

    [ForeignKey("LocationId")]
    public Location Location { get; set; }

    public bool IsEnabled { get; set; }

    [ForeignKey("CreatedByUser")]
    public long CreatedByUserId { get; set; } 
    public User CreatedByUser { get; set; }

    public ICollection<Room> Rooms { get; set; }
}
