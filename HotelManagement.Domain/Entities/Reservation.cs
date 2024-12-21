using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Reservations")]
public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }
    
    [Required]
    public int NumberOfGuests { get; set; }

    public bool IsConfirmed { get; set; } 

    [ForeignKey("Room")]
    public long RoomId { get; set; } 
    public Room Room { get; set; }

    [ForeignKey("Guest")]
    public long GuestId { get; set; }
    public Guest Guest { get; set; }
}
