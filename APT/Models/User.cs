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
        public DateTime CreatedAt { get; set; }
    }

}