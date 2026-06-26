using Locatic.Application.DTOs;

namespace Locatic.Application.Interfaces{

    public interface IReservationService
    {
        Task<IEnumerable<ReservationDto>> GetAllAsync();

        Task CreateAsync(CreateReservationDto dto);
        
        Task DeleteAsync(int id);
    }


}
