
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;
using Locatic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories{

    public class CarModelRepository : ICarModelRepository
    {
        private readonly LocaticDbContext _context;

        public CarModelRepository(LocaticDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarModel>> GetAllAsync()
        {
            return await _context.CarModels.Include(m => m.Brand).ToListAsync();
        }

        public async Task<CarModel?> GetByIdAsync(int id)
        {
            return await _context.CarModels.Include(m => m.Brand).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(CarModel carModel)
        {
            await _context.CarModels.AddAsync(carModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CarModel carModel)
        {
            _context.CarModels.Update(carModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var model = await _context.CarModels.FindAsync(id);
            if (model != null)
            {
                _context.CarModels.Remove(model);
                await _context.SaveChangesAsync();
            }
        }
    }


}
