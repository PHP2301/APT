using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq; // Thêm Linq để dùng SelectMany

namespace APT.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("fullname")]
        public string FullName { get; set; } = null!;

        [Column("email")]
        public string? Email { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("id_card")]
        public string? IdCard { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("dob")]
        public DateTime? Dob { get; set; }

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("role")]
        public string Role { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("building_id")]
        public int? BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        public virtual Building? Building { get; set; }

        // Quan hệ 1-N với Căn hộ (Chủ hộ)
        [InverseProperty("Resident")]
        public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

        // CÁI NÀY LÀ CỨU CÁNH: Gom xe từ tất cả căn hộ của User
     
        //public virtual ICollection<Vehicle> Vehicles
        //{
        //    get => Apartments?.SelectMany(a => a.Vehicles).ToList() ?? new List<Vehicle>();
        //    set { /* Không cần xử lý set */ }
        //}
    }
    
}