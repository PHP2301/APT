using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    [Table("residents")]
    public class Resident
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("fullname")]
        // FIX: Thêm = string.Empty để tránh lỗi "Non-nullable property"
        public string FullName { get; set; } = string.Empty;

        [Column("building_id")]
        public int building_id { get; set; }

        [Column("phone")]
        public string? phone { get; set; }

        [Column("id_card")]
        public string? id_card { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("dob")]
        public DateTime? dob { get; set; }

        [Column("gender")]
        public string? gender { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        // --- QUAN HỆ (Navigation Properties) ---
        // FIX: Đảm bảo có dấu ? để báo là các quan hệ này có thể null khi chưa Include

        [ForeignKey("building_id")]
        public virtual Building? Building { get; set; }

        [ForeignKey("user_id")]
        public virtual User? User { get; set; }

        // FIX: Khởi tạo List ngay lập tức để tránh lỗi null khi gọi Count
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}