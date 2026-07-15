using Locatic.Domain.Entities;





namespace Locatic.Domain.Interfaces
{


    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();

        Task<Reservation?> GetByIdAsync(int id);

        Task AddAsync(Reservation reservation);

        Task DeleteAsync(int id);

        Task<bool> IsCarAvailableAsync(int carId, DateTime start, DateTime end);
    }


}