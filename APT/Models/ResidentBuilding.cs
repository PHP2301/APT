using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class ResidentBuilding
    {
        public int Id { get; set; }

        [Column("resident_id")]
        public int ResidentId { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Resident Resident { get; set; } = null!;

        public Building Building { get; set; } = null!;
    }
}