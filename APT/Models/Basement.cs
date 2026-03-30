namespace APT.Models
{
    public class Basement
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string Name { get; set; }

        public int MaxMotorbikes { get; set; }
        public int MaxCars { get; set; }
        public decimal FeeMotorbike { get; set; }
        public decimal FeeCar { get; set; }

        public Building Building { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}


