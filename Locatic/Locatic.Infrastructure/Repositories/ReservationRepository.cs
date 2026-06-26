using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;
using Locatic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Locatic.Infrastructure.Repositories{

    public class ReservationRepository : IReservationRepository
    {
        private readonly LocaticDbContext _context;

        public ReservationRepository(LocaticDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Client)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsCarAvailableAsync(int carId, DateTime start, DateTime end)
        {
            return !await _context.Reservations.AnyAsync(r =>
                r.CarId == carId &&
                r.StartDate < end &&
                r.EndDate > start);
        }
    }


}
