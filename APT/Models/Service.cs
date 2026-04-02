using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APT.Models
{
    [Table("services")]
    public class Service
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("building_id")]
        public int building_id { get; set; }

        [Column("service_name")]
        public string ServiceName { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("unit")]
        public string? Unit { get; set; }

        // Biến phụ để tránh lỗi build ở các view cũ dùng .Name
        [NotMapped]
        public string Name { get => ServiceName; set => ServiceName = value; }

        [ForeignKey("building_id")]
        public virtual Building? Building { get; set; }
    }
}