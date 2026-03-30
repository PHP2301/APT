namespace APT.Models
{
    public class UtilityReading
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }

        public string Month { get; set; }

        public int ElecOld { get; set; }

        public int ElecNew { get; set; }

        public int WaterOld { get; set; }

        public int WaterNew { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Apartment Apartment { get; set; }
    }
}