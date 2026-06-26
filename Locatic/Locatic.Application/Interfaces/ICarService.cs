using Locatic.Application.DTOs;
namespace Locatic.Application.Interfaces{
    public interface ICarService
    {
        Task<IEnumerable<CarDto>> GetAllAsync();
        Task<CarDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateCarDto dto);
        Task UpdateAsync(int id, CreateCarDto dto);
        Task DeleteAsync(int id);
    }
}
