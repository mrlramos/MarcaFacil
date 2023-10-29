using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("item")]
    public class Item
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Amount")]
        public int Amount { get; set; }

        [Required]
        [ForeignKey("place")]
        [Column("PlaceId")]
        public Guid PlaceId { get; set; }

        [Column("ImagePath")]
        public Guid? ImagePath { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [Column("CreationDate")]
        public DateTime CreationDate { get; set; }
    }
}
