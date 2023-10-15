using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("place")]
    public class Place
    {
        [Key]
        [Column("Id")]
        public Guid? Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Code")]
        public Guid Code { get; set; }

        [NotMapped]
        public User User { get; set; }

        public ICollection<Item>? Items { get; set; }

        [Column("CreationDate")]
        public DateTime? CreationDate { get; set; }

    }
}
