using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

[Table("Rooms")]
public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, MaxLength(50)]
    public string RoomType { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal Taxes { get; set; }

    [Required, MaxLength(50)]
    public string Location { get; set; }

    [Required, DefaultValue(1)]
    public int MaxNumberOfGuest { get; set; }

    public bool IsAvailable { get; set; }

    [ForeignKey("Hotel")]
    public long? HotelId { get; set; }
    public Hotel Hotel { get; set; }

    [ForeignKey("CreatedByUser")] 
    public long CreatedByUserId { get; set; } 
    public User CreatedByUser { get; set; }
}