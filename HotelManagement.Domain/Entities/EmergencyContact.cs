using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("EmergencyContacts")]
public class EmergencyContact
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; }

    [Required, MaxLength(15)]
    public string Phone { get; set; }

    [ForeignKey("Reservations")]
    public long ReservationId { get; set; }
    public Reservation Reservation { get; set; }
}
