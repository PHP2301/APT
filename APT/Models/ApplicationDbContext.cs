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
        public DbSet<Basement> Basements { get; set; }
        //public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<ApartmentService> ApartmentServices { get; set; }
        public DbSet<UtilityReading> UtilityReadings { get; set; }
        public DbSet<UtilityPrice> UtilityPrices { get; set; }
        public DbSet<Bill> Bills { get; set; }

        public DbSet<Building> buildings { get; set; }
        public DbSet<Apartment> apartments { get; set; }

        public DbSet<APT.Models.Service> services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Vehicle>(entity =>
            //{
            //    entity.ToTable("vehicles");
            //    entity.Property(v => v.Id).HasColumnName("id");
            //    entity.Property(v => v.ApartmentId).HasColumnName("apartment_id");
            //    entity.Property(v => v.building_id).HasColumnName("building_id");

            //    // Tuyệt đối KHÔNG khai báo bất kỳ cái gì liên quan đến BasementId hay ResidentId ở đây!
            //});

            // 2. Cấu hình bảng Apartments (Mở lại Resident để hiện tên chủ hộ)
            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.ToTable("apartments");
                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.ResidentId).HasColumnName("resident_id");
                entity.Property(a => a.building_id).HasColumnName("building_id");

                // KHÔNG dùng entity.Ignore("Resident") nếu muốn hiện tên cư dân
            });

            // 3. Đảm bảo các bảng khác dùng tên viết thường
            modelBuilder.Entity<User>(entity => entity.ToTable("users"));
            modelBuilder.Entity<Building>(entity => entity.ToTable("buildings"));
            modelBuilder.Entity<Basement>(entity => entity.ToTable("basements"));
            modelBuilder.Entity<Service>(entity => entity.ToTable("services"));
            modelBuilder.Entity<Bill>(entity => entity.ToTable("bills"));
        }
    }
    }