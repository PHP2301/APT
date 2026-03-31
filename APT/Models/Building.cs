using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Building
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [Column("room_prefix")]
        public string? RoomPrefix { get; set; }

        public string? Address { get; set; }

        public string? Image { get; set; }

        [Column("total_floors")]
        public int TotalFloors { get; set; }

        [Column("total_basements")]
        public int TotalBasements { get; set; }

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

        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
        public ICollection<Basement> Basements { get; set; } = new List<Basement>();
        public ICollection<Resident> Residents { get; set; } = new List<Resident>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}