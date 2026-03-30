namespace APT.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        public int Building_id { get; set; }

        public int FloorNumber { get; set; }

        public string RoomType { get; set; }

        public string RoomNumberSuffix { get; set; }

        public string FullRoomName { get; set; }

        public decimal Area { get; set; }

        public decimal RentPrice { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public int? ResidentId { get; set; }

        public int ManagementPrice { get; set; }

        public Building Building { get; set; }

        public Resident Resident { get; set; }

        public ICollection<ApartmentService> ApartmentServices { get; set; }

        public ICollection<UtilityReading> UtilityReadings { get; set; }
    }
}