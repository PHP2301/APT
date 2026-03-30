namespace APT.Models
{
    public class Building
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RoomPrefix { get; set; }

        public string Address { get; set; }

        public string Image { get; set; }

        public int TotalFloors { get; set; }

        public int TotalBasements { get; set; }

        public int MaxMotorbikes { get; set; }

        public int MaxCars { get; set; }

        public decimal FeeMotorbike { get; set; }

        public decimal FeeCar { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Apartment> Apartments { get; set; }

        public ICollection<Basement> Basements { get; set; }

        public ICollection<Resident> Residents { get; set; }

        public ICollection<Service> Services { get; set; }
    }
}