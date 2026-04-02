//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace APT.Models
//{
//    [Table("vehicles")]
//    public class Vehicle
//    {
//        [Key]
//        [Column("id")]
//        public int Id { get; set; }

//        [Column("apartment_id")]
//        public int ApartmentId { get; set; }

//        [Column("building_id")]
//        public int building_id { get; set; }

//        [Column("vehicle_name")]
//        [Required(ErrorMessage = "Tên xe không được để trống")]
//        public string VehicleName { get; set; } = null!;

//        [Column("license_plate")]
//        [Required(ErrorMessage = "Biển số không được để trống")]
//        public string LicensePlate { get; set; } = null!;

//        [Column("vehicle_type")]
//        public string VehicleType { get; set; } = "Xe máy";

//        [Column("created_at")]
//        public DateTime CreatedAt { get; set; } = DateTime.Now;

//        // Quan hệ chỉ trỏ về Căn hộ và Tòa nhà
//        [ForeignKey("ApartmentId")]
//        public virtual Apartment? Apartment { get; set; }

//        [ForeignKey("building_id")]
//        public virtual Building? Building { get; set; }
//    }
//}