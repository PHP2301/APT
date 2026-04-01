using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace APT.Models
{
    [Table("apartmentservices")] // Đảm bảo trỏ đúng tên bảng viết thường
    public class ApartmentService
    {
        public int Id { get; set; }

        [Column("apartment_id")]
        public int ApartmentId { get; set; }

        [Column("service_id")]
        public int ServiceId { get; set; }

        // SỬA TẠI ĐÂY: Kết hợp cả hai làm một
        [Column("registered_at")]
        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        // Nếu trong Code (Controller/View) Đại Ca lỡ gọi "RegistrationDate", 
        // hãy dùng thuộc tính giả này để trỏ về RegisteredAt mà không tạo thêm cột trong DB
        [NotMapped]
        public DateTime RegistrationDate
        {
            get => RegisteredAt;
            set => RegisteredAt = value;
        }

        // Navigation
        [ForeignKey("ApartmentId")]
        public Apartment Apartment { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public Service Service { get; set; } = null!;
    }
}