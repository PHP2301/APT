using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;

        [Column("created_at")]
        // Tự động gán thời gian hiện tại nếu không có giá trị truyền vào
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}