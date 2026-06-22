using Locatic.Domain.Entities;

namespace Locatic.Domain.Interfaces{

    public interface ICarModelRepository
    {
        Task<IEnumerable<CarModel>> GetAllAsync();

        Task<CarModel?> GetByIdAsync(int id);

        Task AddAsync(CarModel carModel);

        Task UpdateAsync(CarModel carModel);

        Task DeleteAsync(int id);
    }


}
