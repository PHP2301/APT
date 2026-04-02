using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace APT.Models
{
    [Table("buildings")] // Đảm bảo trỏ đúng tên bảng viết thường trong MySQL
    public class Building
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("room_prefix")]
        public string? RoomPrefix { get; set; }

        [Column("address")] // Nên map viết thường cho đồng bộ
        public string? Address { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        // MẸO: Thêm dấu ? để biến thành kiểu Nullable. 
        // Nếu DB không có các cột này, EF sẽ không báo lỗi "Dữ liệu không hợp lệ" nữa.
        [Column("total_floors")]
        public int? TotalFloors { get; set; }

        [Column("total_basements")]
        public int? TotalBasements { get; set; }

        // DÙNG [NotMapped] cho những thứ Đại Ca bảo "không có" 
        // để EF Core không bao giờ SELECT chúng từ DB nữa.
        [NotMapped]
        public int MaxMotorbikes { get; set; }

        [NotMapped]
        public int MaxCars { get; set; }

        [NotMapped]
        public decimal FeeMotorbike { get; set; }

        [NotMapped]
        public decimal FeeCar { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        // Các quan hệ giữ nguyên nhưng thêm virtual để lazy loading mượt hơn
        public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
        public virtual ICollection<Basement> Basements { get; set; } = new List<Basement>();
        public virtual ICollection<User> Residents { get; set; } = new List<User>();
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}