using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("place")]
    public class Place
    {
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Code")]
        public Guid Code { get; set; }

        // Relação com User
        [NotMapped]
        public User User { get; set; }

        // Relação com Item
        [NotMapped]
        public ICollection<Item> Items { get; set; }

        [Column("CreationDate")]
        public DateTime CreationDate { get; set; }

    }
}
