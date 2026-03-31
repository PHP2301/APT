using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Column("resident_id")]
        public int ResidentId { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("basement_id")]
        public int BasementId { get; set; }

        [Column("vehicle_type")]
        public string VehicleType { get; set; } = null!;

        [Column("license_plate")]
        public string? LicensePlate { get; set; }

        public string? Brand { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Resident Resident { get; set; } = null!;
        public Basement Basement { get; set; } = null!;
        public Building Building { get; set; } = null!;
    }
}   