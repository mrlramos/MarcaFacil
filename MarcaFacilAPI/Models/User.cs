using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Email")]
        public string Email { get; set; }

        [Required]
        [Column("Password")]
        public string Password { get; set; }

        [Column("CreationDate")]
        public DateTime CreationDate { get; set; }
    }
}
