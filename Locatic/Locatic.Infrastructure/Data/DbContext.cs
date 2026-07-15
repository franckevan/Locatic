using Locatic.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Locatic.Infrastructure.Data
{

    public class LocaticDbContext : DbContext
    {
        public LocaticDbContext(DbContextOptions<LocaticDbContext> options)
            : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<CarModel> CarModels { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

    }

}