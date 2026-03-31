using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Resident
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        public string Fullname { get; set; } = null!;

        public string? Phone { get; set; }

        [Column("id_card")]
        public string? IdCard { get; set; }

        public string? Email { get; set; }

        public DateTime Dob { get; set; }

        public string Gender { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;

        public Building Building { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; }
            = new List<Vehicle>();
    }
}