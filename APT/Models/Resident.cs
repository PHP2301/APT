namespace APT.Models
{
    public class Resident
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BuildingId { get; set; }

        public string Fullname { get; set; }

        public string Phone { get; set; }

        public string IdCard { get; set; }

        public string Email { get; set; }

        public DateTime Dob { get; set; }

        public string Gender { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; }

        public Building Building { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}