using System.ComponentModel.DataAnnotations.Schema;

namespace APT.Models
{
    public class ServicePrice
    {
        public int Id { get; set; }

        [Column("building_id")]
        public int BuildingId { get; set; }

        [Column("service_name")]
        public string ServiceName { get; set; } = null!;

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("unit_name")]
        public string? UnitName { get; set; }

        [Column("is_mandatory")]
        public bool IsMandatory { get; set; }

        public Building Building { get; set; } = null!;
    }
}