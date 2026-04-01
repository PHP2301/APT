using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace APT.Models
{
    [Table("services")] // Đảm bảo trỏ đúng tên bảng trong MySQL
    public class Service
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        // MẸO: Thêm thuộc tính này để "cứu" 6 lỗi Build ở các Controller
        // Nó trỏ thẳng về BuildingId nên dữ liệu luôn đồng bộ
        [NotMapped]
        public int building_id
        {
            get => BuildingId;
            set => BuildingId = value;
        }

        // Navigation
        [ForeignKey("BuildingId")]
        public Building? Building { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Unit { get; set; }
    }
}