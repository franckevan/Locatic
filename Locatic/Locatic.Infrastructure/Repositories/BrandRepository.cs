using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;
using Locatic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories
{

    public class BrandRepository : IBrandRepository
    {
        private readonly LocaticDbContext _context;

        public BrandRepository(LocaticDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task AddAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }
    }


}
