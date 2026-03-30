using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        [Column("building_id")] // Báo cho EF biết cột dưới SQL tên là building_id
        public int? BuildingId { get; set; }
    }

}