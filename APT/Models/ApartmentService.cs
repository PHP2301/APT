namespace APT.Models
{
    public class ApartmentService
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }

        public int ServiceId { get; set; }

        public DateTime RegisteredAt { get; set; }

        public Apartment Apartment { get; set; }

        public Service Service { get; set; }
    }
}