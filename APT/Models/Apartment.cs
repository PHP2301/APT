using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace APT.Models
{
    public class Apartment
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int building_id { get; set; }

        [NotMapped]
        public int BuildingId { get => building_id; set => building_id = value; }

        [ForeignKey("building_id")]
        public virtual Building? Building { get; set; }

        [Column("floor_number")]
        public int FloorNumber { get; set; }

        // --- ROOM TYPE ---
        [Column("room_type")]
        public string RoomType { get; set; } = "Studio";

        [NotMapped] // Cứu lỗi: 'Apartment' does not contain a definition for 'room_type'
        public string room_type { get => RoomType; set => RoomType = value; }

        [Column("room_number_suffix")]
        public string RoomNumberSuffix { get; set; } = null!;

        // --- FULL ROOM NAME ---
        [Column("full_room_name")]
        public string FullRoomName { get; set; } = null!;

        [NotMapped] // Cứu lỗi: 'Apartment' does not contain a definition for 'full_room_name'
        public string full_room_name { get => FullRoomName; set => FullRoomName = value; }

        public decimal? Area { get; set; }

        // --- RENT PRICE ---
        [Column("rent_price")]
        public decimal? RentPrice { get; set; }

        [NotMapped] // Cứu lỗi: 'Apartment' does not contain a definition for 'rent_price'
        public decimal? rent_price { get => RentPrice; set => RentPrice = value; }

        public string Status { get; set; } = "Available";
        public string? Description { get; set; }

        [Column("resident_id")]
        public int? ResidentId { get; set; }

        [ForeignKey("ResidentId")]
        public virtual User? Resident { get; set; } // Đổi thành User? để hết lỗi Resident/User

        [Column("management_price")]
        public int? ManagementPrice { get; set; }

        public virtual ICollection<ApartmentService> ApartmentServices { get; set; } = new List<ApartmentService>();
        public virtual ICollection<UtilityReading> UtilityReadings { get; set; } = new List<UtilityReading>();
    }
}