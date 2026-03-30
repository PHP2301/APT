namespace APT.Models
{
    public class BuildingManager
    {
        public int Id { get; set; }
        //public int BuildingId { get; set; }
        public int ManagerId { get; set; }

        public Building Building { get; set; }
        public User Manager { get; set; }
    }
}
