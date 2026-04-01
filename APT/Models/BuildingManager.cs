using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    [Table("building_managers")]
    public class BuildingManager
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("manager_id")]
        public int ManagerId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        [ForeignKey("BuildingId")]
        public Building Building { get; set; } = null!;

        [ForeignKey("ManagerId")]
        public User Manager { get; set; } = null!;
    }
}