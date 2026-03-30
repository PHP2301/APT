using Microsoft.EntityFrameworkCore;
using APT.Models;

namespace APT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Basement> Basements { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<ApartmentService> ApartmentServices { get; set; }
        public DbSet<UtilityReading> UtilityReadings { get; set; }
        public DbSet<UtilityPrice> UtilityPrices { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BuildingManager> BuildingManagers { get; set; }
        public DbSet<ManagerAssignment> ManagerAssignments { get; set; }
        public DbSet<ResidentBuilding> ResidentBuildings { get; set; }
    }
}