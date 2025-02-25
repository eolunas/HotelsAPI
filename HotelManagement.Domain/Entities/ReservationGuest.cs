using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("ReservationGuests")]
public class ReservationGuest
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("Reservation")]
    public long ReservationId { get; set; }
    public Reservation Reservation { get; set; }

    [ForeignKey("Guest")]
    public long GuestId { get; set; }
    public Guest Guest { get; set; }
}
