namespace MarcaFacilAPI.Models
{
    public class Place
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Code { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid UserId { get; set; }
    }
}
