namespace APT.Models
{
    public class Bill
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public string Month { get; set; }

        public int TotalMoney { get; set; }

        public bool Status { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Apartment Apartment { get; set; }
    }
}