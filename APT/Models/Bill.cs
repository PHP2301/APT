using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class Bill
    {
        public int Id { get; set; }

        [Column("room_id")]
        public int RoomId { get; set; }

        public string Month { get; set; } = null!;

        [Column("total_money")]
        public int TotalMoney { get; set; }

        public bool Status { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        [ForeignKey("RoomId")]
        public Apartment Apartment { get; set; } = null!;
    }
}