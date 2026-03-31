using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Basement
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        public string Name { get; set; } = null!;

        [Column("max_motorbikes")]
        public int MaxMotorbikes { get; set; }

        [Column("max_cars")]
        public int MaxCars { get; set; }

        [Column("fee_motorbike")]
        public decimal FeeMotorbike { get; set; }

        [Column("fee_car")]
        public decimal FeeCar { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Building Building { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; }
            = new List<Vehicle>();
    }
}