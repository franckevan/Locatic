using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Locatic.Infrastructure.Data
{

    public class LocaticDbContextFactory : IDesignTimeDbContextFactory<LocaticDbContext>
    {
        public LocaticDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LocaticDbContext>();

            optionsBuilder.UseSqlite("Data Source=locatic.db");

            return new LocaticDbContext(optionsBuilder.Options);
        }
    }


}