namespace APT.Models
{
    public class ManagerAssignment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BuildingId { get; set; }

        public DateTime AssignedAt { get; set; }

        public User User { get; set; }

        public Building Building { get; set; }
    }
}