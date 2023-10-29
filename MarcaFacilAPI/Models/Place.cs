using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("place")]
    public class Place
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Code")]
        public Guid Code { get; set; }

        [Required]
        [ForeignKey("user")]
        [Column("UserId")]
        public Guid UserId { get; set; }

        [Column("ImagePath")]
        public Guid? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        [Column("CreationDate")]
        public DateTime CreationDate { get; set; }

    }
}
