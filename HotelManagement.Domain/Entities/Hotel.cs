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

    [Required, MaxLength(100)]
    public string Location { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }

    public bool IsEnabled { get; set; }

    [ForeignKey("CreatedByUser")]
    public long CreatedByUserId { get; set; } 
    public User CreatedByUser { get; set; }

    public ICollection<Room> Rooms { get; set; }
}
