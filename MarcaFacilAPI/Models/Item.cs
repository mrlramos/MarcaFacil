using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("item")]
    public class Item
    {
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Amount")]
        public int Amount { get; set; }

        // Relação com Place
        public Place place { get; set; }

        [Column("CreationDate")]
        public DateTime CreationDate { get; set; }
    }
}
