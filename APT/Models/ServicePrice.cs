namespace APT.Models
{
    public class ServicePrice
    {
        public int Id { get; set; }

        public int BuildingId { get; set; }

        public string ServiceName { get; set; }

        public decimal UnitPrice { get; set; }

        public string UnitName { get; set; }

        public bool IsMandatory { get; set; }

        public Building Building { get; set; }
    }
}