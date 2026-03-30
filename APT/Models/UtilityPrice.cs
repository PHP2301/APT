namespace APT.Models
{
    public class UtilityPrice
    {
        public int Id { get; set; }

        public int BuildingId { get; set; }

        public int PriceLv1 { get; set; }

        public int PriceLv2 { get; set; }

        public int PriceLv3 { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Building Building { get; set; }
    }
}