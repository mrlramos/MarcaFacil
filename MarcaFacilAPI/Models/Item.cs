namespace MarcaFacilAPI.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid PlaceId { get; set; }
    }
}
