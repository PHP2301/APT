using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class ManagerAssignment
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int user_id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("assigned_at")]
        public DateTime AssignedAt { get; set; }

        // Navigation
        [ForeignKey("user_id")]
        public User User { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }
    }
}