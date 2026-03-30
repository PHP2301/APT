namespace APT.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int ResidentId { get; set; }
        public int BuildingId { get; set; }
        public int BasementId { get; set; }

        public string VehicleType { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public DateTime CreatedAt { get; set; }

        public Resident Resident { get; set; }
        public Basement Basement { get; set; }
        public Building Building { get; set; }
    }

}
