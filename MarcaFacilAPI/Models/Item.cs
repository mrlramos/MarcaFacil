using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcaFacilAPI.Models
{
    [Table("item")]
    public class Item
    {
        [Key]
        [Column("Id")]
        public Guid? Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Amount")]
        public int Amount { get; set; }

        public Place place { get; set; }

        [Column("CreationDate")]
        public DateTime? CreationDate { get; set; }
    }
}
