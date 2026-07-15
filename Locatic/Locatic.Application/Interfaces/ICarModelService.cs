using Locatic.Application.DTOs;

namespace Locatic.Application.Interfaces
{

    public interface ICarModelService
    {
        Task<IEnumerable<CarModelDto>> GetAllAsync();

        Task<CarModelDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateCarModelDto dto);

        Task UpdateAsync(int id, CreateCarModelDto dto);


        Task DeleteAsync(int id);
    }


}