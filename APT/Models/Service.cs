namespace APT.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public string Unit { get; set; }
    }
}
