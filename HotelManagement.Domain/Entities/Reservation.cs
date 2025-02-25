using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Reservations")]
public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public DateOnly CheckInDate { get; set; }

    [Required]
    public DateOnly CheckOutDate { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Number of Guests must be at least 1")]
    public int NumberOfGuests { get; set; }

    public bool IsConfirmed { get; set; }

    [ForeignKey("Hotel")]
    public long HotelId { get; set; }
    public Hotel Hotel { get; set; }

    [ForeignKey("Room")]
    public long RoomId { get; set; } 
    public Room Room { get; set; }

    public List<ReservationGuest> ReservationGuests { get; set; } = new List<ReservationGuest>();

    [NotMapped]
    public int Nights => (CheckOutDate.DayNumber - CheckInDate.DayNumber);
}
