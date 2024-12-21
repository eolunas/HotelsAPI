using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Guests")]
public class Guest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [Required, MaxLength(10)]
    public string Gender { get; set; }

    [Required, MaxLength(20)]
    public string DocumentType { get; set; }

    [Required, MaxLength(50)]
    public string DocumentNumber { get; set; }

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; }

    [Required, MaxLength(15)]
    public string Phone { get; set; }

}
