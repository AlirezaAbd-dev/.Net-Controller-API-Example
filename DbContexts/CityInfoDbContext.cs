using Cityinfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cityinfo.API.DbContexts
{
    public class CityInfoDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        public DbSet<User> Users { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //    optionsBuilder.UseSqlite();
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                    new City("Guilan")
                    {
                        Id = 2,
                        Description = "Best city in the world",
                        UserId = 1
                    },
                    new City("Tehran")
                    {
                        Id = 3,
                        Description = "Best city in the world",
                        UserId = 1
                    },
                    new City("Shiraz")
                    {
                        Id = 4,
                        Description = "Best city in the world",
                        UserId = 1
                    }
                );

            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                    new PointOfInterest("bandar anzali")
                    {
                        Id = 1,
                        CityId = 2,
                        Description = "Best bandar in the world"
                    },
                    new PointOfInterest("bandar ganaveh")
                    {
                        Id = 2,
                        CityId = 3,
                        Description = "Best bandar in the world"
                    },
                    new PointOfInterest("bandar Imam khomeini")
                    {
                        Id = 3,
                        CityId = 3,
                        Description = "Best bandar in the world"
                    },
                    new PointOfInterest("bandar yoyo")
                    {
                        Id = 4,
                        CityId = 2,
                        Description = "Best bandar in the world"
                    }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
