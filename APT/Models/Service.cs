using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Unit { get; set; }

        // Navigation
        public Building Building { get; set; } = null!;
    }
}