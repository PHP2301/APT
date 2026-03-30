namespace APT.Models
{
    public class ResidentBuilding
    {
        public int Id { get; set; }

        public int ResidentId { get; set; }

        public int BuildingId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Resident Resident { get; set; }

        public Building Building { get; set; }
    }
}