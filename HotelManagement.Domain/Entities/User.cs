using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; }

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; }

    [Required]
    public UserRole Role { get; set; }

    public ICollection<Hotel> CreatedHotels { get; set; }
    public ICollection<Room> CreatedRooms { get; set; }
}

