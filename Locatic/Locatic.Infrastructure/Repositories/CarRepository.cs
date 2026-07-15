using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;
using Locatic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Locatic.Infrastructure.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly LocaticDbContext _context;
        public CarRepository(LocaticDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars
                .Include(c => c.CarModel)
                .ThenInclude(m => m.Brand)
                .ToListAsync();
        }
        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarModel)
                .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}