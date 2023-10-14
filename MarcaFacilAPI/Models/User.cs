using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("Id")]
        public Guid? Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Password")]
        public string Password { get; set; }

        [NotMapped]
        public ICollection<Place>? Places { get; set; }

        [Column("CreationDate")]
        public DateTime? CreationDate { get; set; }
    }
}
