using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class ApartmentService
    {
        public int Id { get; set; }

        [Column("apartment_id")]
        public int ApartmentId { get; set; }

        [Column("service_id")]
        public int ServiceId { get; set; }

        [Column("registered_at")]
        public DateTime RegisteredAt { get; set; }

        // Navigation
        [ForeignKey("ApartmentId")]
        public Apartment Apartment { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public Service Service { get; set; } = null!;
    }
}