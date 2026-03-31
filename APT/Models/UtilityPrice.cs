using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class UtilityPrice
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("price_lv1")]
        public int PriceLv1 { get; set; }

        [Column("price_lv2")]
        public int PriceLv2 { get; set; }

        [Column("price_lv3")]
        public int PriceLv3 { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Building Building { get; set; } = null!;
    }
}