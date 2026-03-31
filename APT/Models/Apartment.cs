using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Apartment
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("floor_number")]
        public int FloorNumber { get; set; }

        [Column("room_type")]
        public string RoomType { get; set; } = null!;

        [Column("room_number_suffix")]
        public string RoomNumberSuffix { get; set; } = null!;

        [Column("full_room_name")]
        public string FullRoomName { get; set; } = null!;

        public decimal Area { get; set; }

        [Column("rent_price")]
        public decimal RentPrice { get; set; }

        public string Status { get; set; } = null!;

        public string? Description { get; set; }

        [Column("resident_id")]
        public int? ResidentId { get; set; }

        [Column("management_price")]
        public int ManagementPrice { get; set; }

        public Building Building { get; set; } = null!;
        public Resident? Resident { get; set; }

        public ICollection<ApartmentService> ApartmentServices { get; set; }
            = new List<ApartmentService>();

        public ICollection<UtilityReading> UtilityReadings { get; set; }
            = new List<UtilityReading>();
    }
}