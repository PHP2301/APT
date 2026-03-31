using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class UtilityReading
    {
        public int Id { get; set; }

        [Column("apartment_id")]
        public int ApartmentId { get; set; }

        public string Month { get; set; } = null!;

        [Column("elec_old")]
        public int ElecOld { get; set; }

        [Column("elec_new")]
        public int ElecNew { get; set; }

        [Column("water_old")]
        public int WaterOld { get; set; }

        [Column("water_new")]
        public int WaterNew { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Apartment Apartment { get; set; } = null!;
    }
}