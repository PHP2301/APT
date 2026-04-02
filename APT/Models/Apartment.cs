using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    [Table("apartments")]
    public class Apartment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("building_id")] // Đảm bảo có dòng này để khớp với MySQL
        public int building_id { get; set; }

        [ForeignKey("building_id")]
        public virtual Building? Building { get; set; }

        [Column("floor_number")]
        public int FloorNumber { get; set; }

        [Column("room_type")]
        public string RoomType { get; set; } = "Studio";

        [Column("room_number_suffix")]
        public string RoomNumberSuffix { get; set; } = null!;

        [Column("full_room_name")]
        public string FullRoomName { get; set; } = null!;

        [Column("area")]
        public decimal? Area { get; set; }

        [Column("rent_price")]
        public decimal? RentPrice { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Available";

        [Column("description")]
        public string? Description { get; set; }

        [Column("resident_id")]
        public int? ResidentId { get; set; }

        [ForeignKey("ResidentId")]
        [InverseProperty("Apartments")] // Phải khớp với tên Collection trong User.cs
        public virtual User? Resident { get; set; }

        [Column("management_price")]
        public int? ManagementPrice { get; set; }

        public virtual ICollection<ApartmentService> ApartmentServices { get; set; } = new List<ApartmentService>();
        public virtual ICollection<UtilityReading> UtilityReadings { get; set; } = new List<UtilityReading>();
        // Trong file Models/Apartment.cs
        //public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}